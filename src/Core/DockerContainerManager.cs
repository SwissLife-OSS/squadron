using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Polly;
using Version = System.Version;

namespace Squadron;

/// <summary>
/// Manager to work with docker containers
/// </summary>
/// <seealso cref="Squadron.IDockerContainerManager" />
public class DockerContainerManager : IDockerContainerManager
{
    private static readonly IDictionary<string, string> Networks = new Dictionary<string, string>();
    private static readonly SemaphoreSlim SyncNetworks = new(1, 1);

    private readonly ContainerResourceSettings _settings;
    private readonly DockerConfiguration _dockerConfiguration;
    private readonly AuthConfig _authConfig = null;

    private readonly VariableResolver _variableResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="DockerContainerManager"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <param name="dockerConfiguration"></param>
    public DockerContainerManager(ContainerResourceSettings settings,
        DockerConfiguration dockerConfiguration)
    {
        _settings = settings;
        _dockerConfiguration = dockerConfiguration;
        Client = new DockerClientConfiguration(
                LocalDockerUri(),
                null,
                TimeSpan.FromMinutes(5)
            )
            .CreateClient();
        _authConfig = GetAuthConfig();
        _variableResolver = new VariableResolver(_settings.Variables);
    }

    public ContainerInstance Instance { get; } = new ContainerInstance();

    public DockerClient Client { get; }

    private AuthConfig GetAuthConfig()
    {
        if (_settings.RegistryName != null)
        {
            DockerRegistryConfiguration? registryConfig = _dockerConfiguration
                .Registries
                .FirstOrDefault(x => x.Name.Equals(
                    _settings.RegistryName,
                    StringComparison.InvariantCultureIgnoreCase));

            if (registryConfig == null)
            {
                throw new InvalidOperationException(
                    $"No container registry with name '{_settings.RegistryName}'" +
                    "found in configuration");
            }

            return GetAuthConfig(registryConfig);
        }

        return TrySetDefaultAuthConfig(_settings.Image);
    }

    private AuthConfig GetAuthConfig(DockerRegistryConfiguration registryConfig)
    {
        return new AuthConfig
        {
            Username = string.IsNullOrEmpty(registryConfig.Username)?null:registryConfig.Username,
            Password = string.IsNullOrEmpty(registryConfig.Password)?null:registryConfig.Password,
            ServerAddress = registryConfig.Address
        };
    }

    private AuthConfig TrySetDefaultAuthConfig(string imageName)
    {
        var registryName = "index.docker.io";

        try
        {
            registryName = new Uri(imageName).Host;
        }
        catch{}

        DockerRegistryConfiguration? registryConfig = _dockerConfiguration
            .Registries
            .FirstOrDefault(x => x.Name.Equals(
                registryName,
                StringComparison.InvariantCultureIgnoreCase));

        return registryConfig != null ? GetAuthConfig(registryConfig) : new AuthConfig();
    }

    private static Uri LocalDockerUri()
    {
        var envHost = Environment.GetEnvironmentVariable("DOCKER_HOST");

        if (!string.IsNullOrEmpty(envHost))
        {
            return new Uri(envHost);
        }

#if NET461
            return new Uri("npipe://./pipe/docker_engine");
#else
        var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        return isWindows ? new Uri("npipe://./pipe/docker_engine") : new Uri("unix:/var/run/docker.sock");
#endif
    }

    /// <inheritdoc/>
    public async Task CreateAndStartContainerAsync()
    {
        if (!_settings.PreferLocalImage || !await ImageExists())
        {
            await PullImageAsync();
        }

        await CreateContainerAsync();
        await StartContainerAsync();
        await ConnectToNetworksAsync();
        await ResolveHostAddressAsync();
        await CreateLogStreamAsync();

        if (!Instance.IsRunning)
        {
            await ConsumeLogsAsync(TimeSpan.FromSeconds(5));
            throw new ContainerException("Container could not be started (see test output).");
        }
    }

    /// <inheritdoc/>
    public async Task<bool> StopContainerAsync()
    {
        var stopOptions = new ContainerStopParameters { WaitBeforeKillSeconds = 5 };

        bool stopped = await Client.Containers
            .StopContainerAsync(Instance.Id, stopOptions, default);

        if (stopped)
        {
            _settings.Logger.Information("Container stopped");
        }

        return stopped;
    }

    public async Task PauseAsync(TimeSpan resumeAfter)
    {
        await Client.Containers.PauseContainerAsync(Instance.Id);
        Task.Delay(resumeAfter)
            .ContinueWith((c) => ResumeAsync())
            .Start();
    }

    public async Task ResumeAsync()
    {
        await Client.Containers.UnpauseContainerAsync(Instance.Id);
    }

    /// <inheritdoc/>
    public async Task RemoveContainerAsync()
    {
        var removeOptions = new ContainerRemoveParameters { Force = true, RemoveVolumes = true };

        try
        {
            await Retry(async () =>
            {
                await Client.Containers
                    .RemoveContainerAsync(Instance.Id, removeOptions);

                foreach (string network in _settings.Networks)
                {
                    await RemoveNetworkIfUnused(network);
                }
            });
        }
        catch (Exception ex)
        {
            throw new ContainerException(
                $"Error in RemoveContainer: {_settings.UniqueContainerName}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task CopyToContainerAsync(CopyContext context, bool overrideTargetName = false)
    {
        await Retry(async () =>
        {
            using var archiver = new TarArchiver(context, overrideTargetName);

            await Client.Containers.ExtractArchiveToContainerAsync(
                Instance.Id,
                new ContainerPathStatParameters
                {
                    AllowOverwriteDirWithFile = true, Path = context.DestinationFolder.Replace("\\", "/")
                }, archiver.Stream);
        });
    }

    /// <inheritdoc/>
    public async Task<string?> InvokeCommandAsync(ContainerExecCreateParameters parameters)
    {
        ContainerExecCreateResponse response = await Client.Exec
            .ExecCreateContainerAsync(Instance.Id, parameters);

        if (string.IsNullOrEmpty(response.ID))
        {
            return null;
        }

        using MultiplexedStream stream =
            await Client.Exec.StartAndAttachContainerExecAsync(response.ID, false);

        (var stdout, var stderr) = await stream
            .ReadOutputToEndAsync(CancellationToken.None);

        if (!string.IsNullOrEmpty(stderr) && stderr.ToLowerInvariant().Contains("error"))
        {
            var error = new StringBuilder();
            var command = string.Join(" ", parameters.Cmd);
            error.AppendLine($"Error when invoking command \"{command}\"");
            error.AppendLine(stderr);

            throw new ContainerException(error.ToString());
        }

        return stdout;
    }

    private async Task CreateLogStreamAsync()
    {
        _settings.Logger.Verbose("Create log stream");

        var containerStatsParameters = new ContainerLogsParameters
        {
            Follow = true, ShowStderr = true, ShowStdout = true
        };

        Instance.LogStream = await Client
            .Containers
            .GetContainerLogsAsync(Instance.Id, containerStatsParameters);
    }

    /// <inheritdoc/>
    public async Task<string> ConsumeLogsAsync(TimeSpan timeout)
    {
        var logs = await ReadAsync(timeout);
        Instance.Logs.Add(logs);
        _settings.Logger.ContainerLogs(logs);
        return logs;
    }

    private async Task StartContainerAsync()
    {
        var containerStartParameters = new ContainerStartParameters();

        try
        {
            await Retry(async () =>
            {
                _settings.Logger.Verbose("Try start container");
                bool started = await Client.Containers.StartContainerAsync(
                    Instance.Id,
                    containerStartParameters);

                if (!started)
                {
                    _settings.Logger.Warning("Container didn't start");
                }
                else
                {
                    _settings.Logger.Information("Container started");
                }
            });
        }
        catch (Exception ex)
        {
            _settings.Logger.Error("Container start failed", ex);
            throw new ContainerException(
                $"Error in StartContainer: {_settings.UniqueContainerName}", ex);
        }
    }

    private async Task CreateContainerAsync()
    {
        ResolveAndReplaceVariables();

        var hostConfig = new HostConfig
        {
            PublishAllPorts = true,
            Memory = _settings.Memory,
            PortBindings = new Dictionary<string, IList<PortBinding>>(),
            Binds = _settings.Volumes
        };

        var allPorts = new List<ContainerPortMapping>
        {
            new ContainerPortMapping()
            {
                InternalPort = _settings.InternalPort,
                ExternalPort = _settings.ExternalPort,
                HostIp = _settings.HostIp
            }
        };
        allPorts.AddRange(_settings.AdditionalPortMappings);

        foreach (ContainerPortMapping containerPortMapping in allPorts)
        {
            var portMapping =
                new KeyValuePair<string, IList<PortBinding>>(
                    containerPortMapping.InternalPort + "/tcp",
                    new List<PortBinding>());

            portMapping.Value.Add(
                new PortBinding()
                {
                    HostIP = containerPortMapping.HostIp ?? "",
                    HostPort = containerPortMapping.ExternalPort != 0
                        ? containerPortMapping.ExternalPort.ToString()
                        : ""
                });

            hostConfig.PortBindings.Add(portMapping);
        }

        var startParams = new CreateContainerParameters
        {
            Name = _settings.UniqueContainerName,
            Image = _settings.ImageFullname,
            AttachStdout = true,
            AttachStderr = true,
            AttachStdin = false,
            Tty = false,
            HostConfig = hostConfig,
            Env = _settings.EnvironmentVariables,
            Cmd = _settings.Cmd,
            ExposedPorts = allPorts.ToDictionary(k => $"{k.InternalPort}/tcp", v => new EmptyStruct()),
        };

        try
        {
            await Retry(async () =>
            {
                _settings.Logger.Verbose("Try create container");
                _settings.Logger.StartParameters(startParams);
                CreateContainerResponse response = await Client
                    .Containers
                    .CreateContainerAsync(startParams);

                if (string.IsNullOrEmpty(response.ID))
                {
                    _settings.Logger.Warning("Container was not created");
                }
                else
                {
                    Instance.Id = response.ID;
                    Instance.Name = startParams.Name;
                }
            });

            foreach (CopyContext copyContext in _settings.FilesToCopy)
            {
                await CopyToContainerAsync(copyContext, true);
            }
        }
        catch (Exception ex)
        {
            _settings.Logger.Error("Container creation failed", ex);
            throw new ContainerException(
                $"Error in CreateContainer: {_settings.UniqueContainerName}", ex);
        }
    }

    private void ResolveAndReplaceVariables()
    {
        ResolveAdditionalPortsVariables();
        ReplaceVariablesInEnvironmentalVariables();
    }

    private void ReplaceVariablesInEnvironmentalVariables()
    {
        foreach (Variable variable in _settings.Variables)
        {
            _settings.EnvironmentVariables = _settings.EnvironmentVariables
                .Select(p => p.Replace(
                    $"{{{variable.Name}}}",
                    _variableResolver.Resolve<string>(variable.Name)))
                .ToList();
        }
    }

    private void ResolveAdditionalPortsVariables()
    {
        foreach (ContainerPortMapping additionalPort in _settings.AdditionalPortMappings)
        {
            if (!string.IsNullOrEmpty(additionalPort.InternalPortVariableName))
            {
                additionalPort.InternalPort = _variableResolver.Resolve<int>(
                    additionalPort.InternalPortVariableName);
            }

            if (!string.IsNullOrEmpty(additionalPort.ExternalPortVariableName))
            {
                additionalPort.ExternalPort = _variableResolver.Resolve<int>(
                    additionalPort.ExternalPortVariableName);
            }
        }
    }

    public async Task<bool> ImageExists()
    {
        try
        {
            return await Retry(async () =>
            {
                IEnumerable<ImagesListResponse> listResponse =
                    await Client.Images.ListImagesAsync(
                        new ImagesListParameters
                        {
                            Filters = new Dictionary<string, IDictionary<string, bool>>
                            {
                                ["reference"] = new Dictionary<string, bool>
                                {
                                    [_settings.ImageFullname] = true
                                }
                            }
                        });

                return listResponse.Any();
            });
        }
        catch (Exception ex)
        {
            throw new ContainerException(
                $"Error in ImageExists: {_settings.ImageFullname}", ex);
        }
    }

    private async Task PullImageAsync()
    {
        void Handler(JSONMessage message)
        {
            if (!string.IsNullOrEmpty(message.ErrorMessage))
            {
                throw new ContainerException(
                    $"Could not pull the image: {_settings.ImageFullname}. " +
                    $"Error: {message.ErrorMessage}");
            }
        }

        try
        {
            await Retry(async () =>
            {
                await Client.Images.CreateImageAsync(
                    new ImagesCreateParameters { FromImage = _settings.ImageFullname },
                    _authConfig,
                    new Progress<JSONMessage>(Handler));
            });
        }
        catch (Exception ex)
        {
            throw new ContainerException(
                $"Error in PullImage: {_settings.ImageFullname}", ex);
        }
    }

    private async Task ResolveHostAddressAsync()
    {
        bool bindingsResolved = false;

        using (var cancellation = new CancellationTokenSource())
        {
            cancellation.CancelAfter(_settings.WaitTimeout);

            while (!cancellation.IsCancellationRequested && !bindingsResolved)
            {
                try
                {
                    ContainerInspectResponse inspectResponse = await Client
                        .Containers
                        .InspectContainerAsync(Instance.Id, cancellation.Token);

                    ContainerAddressMode addressMode = GetAddressMode();

                    if (addressMode == ContainerAddressMode.Port)
                    {
                        Instance.HostPort =
                            ResolvePort(inspectResponse, $"{_settings.InternalPort}/tcp");

                        foreach (ContainerPortMapping portMapping in _settings.AdditionalPortMappings)
                        {
                            Instance.AdditionalPorts.Add(new ContainerPortMapping()
                            {
                                HostIp = portMapping.HostIp,
                                InternalPort = portMapping.InternalPort,
                                ExternalPort = ResolvePort(
                                    inspectResponse,
                                    $"{portMapping.InternalPort}/tcp")
                            });
                        }
                    }
                    else
                    {
                        Instance.Address = inspectResponse.NetworkSettings.IPAddress;
                        Instance.HostPort = _settings.InternalPort;
                    }

                    Instance.IsRunning = inspectResponse.State.Running;

                    bindingsResolved = true;
                }
                catch (Exception ex)
                {
                    _settings.Logger.Error("Container bindings not resolved", ex);
                }
            }
        }

        if (!bindingsResolved)
        {
            throw new ContainerException("Failed to resolve host all bindings.");
        }
    }

    private int ResolvePort(ContainerInspectResponse inspectResponse, string containerPort)
    {
        Instance.Address = "localhost";
        if (!inspectResponse.NetworkSettings.Ports.ContainsKey(containerPort))
        {
            throw new ContainerException($"Failed to resolve host port for {containerPort}");
        }

        PortBinding binding = inspectResponse
            .NetworkSettings
            .Ports[containerPort]
            .FirstOrDefault();

        if (binding == null || string.IsNullOrEmpty(binding.HostPort))
        {
            throw new ContainerException($"The resolved port binding is empty");
        }

        return int.Parse(binding.HostPort);
    }

    private ContainerAddressMode GetAddressMode()
    {
        ContainerAddressMode addressMode = _dockerConfiguration.DefaultAddressMode;
        if (_settings.AddressMode != ContainerAddressMode.Auto)
        {
            //Overide by user setting
            addressMode = _settings.AddressMode;
        }

        if (addressMode == ContainerAddressMode.Auto)
        {
            //Default to port when not defined
            addressMode = ContainerAddressMode.Port;
        }
#if !NET461
        if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            //OSX can only handle Port
            addressMode = ContainerAddressMode.Port;
        }
#endif
        return addressMode;
    }

    private async Task<string> ReadAsync(TimeSpan timeout)
    {
        var result = new StringBuilder();
        var timeoutTask = Task.Delay(timeout);
        const int size = 256;
        byte[] buffer = new byte[size];

        if (Instance.LogStream == null)
        {
            return "No log stream";
        }

        while (true)
        {
            Task<int> readTask = Instance.LogStream.ReadAsync(buffer, 0, size);

            if (await Task.WhenAny(readTask, timeoutTask) == timeoutTask)
            {
                break;
            }

            var read = await readTask;
            if (read <= 0)
            {
                break;
            }

            char[] chunkChars = new char[read * 2];

            int consumed = 0;
            for (int i = 0; i < read; i++)
            {
                if (buffer[i] > 31 && buffer[i] < 128)
                {
                    chunkChars[consumed++] = (char)buffer[i];
                }
                else if (buffer[i] == (byte)'\n')
                {
                    chunkChars[consumed++] = '\r';
                    chunkChars[consumed++] = '\n';
                }
                else if (buffer[i] == (byte)'\t')
                {
                    chunkChars[consumed++] = '\t';
                }
            }

            string chunk = new string(chunkChars, 0, consumed);
            result.Append(chunk);
        }

        return result.ToString();
    }

    private async Task ConnectToNetworksAsync()
    {
        foreach (string networkName in _settings.Networks)
        {
            string networkId = await GetNetworkId(networkName);

            await Retry(async () =>
            {
                await Client.Networks.ConnectNetworkAsync(
                    networkId,
                    new NetworkConnectParameters { Container = Instance.Id });
            });
        }
    }

    private async Task<string> GetNetworkId(string networkName)
    {
        await SyncNetworks.WaitAsync();

        try
        {
            if (Networks.ContainsKey(networkName))
            {
                return Networks[networkName];
            }

            return await CreateNetwork(networkName);
        }
        finally
        {
            SyncNetworks.Release();
        }
    }

    private async Task<string> CreateNetwork(string networkName)
    {
        string uniqueNetworkName = UniqueNameGenerator.CreateNetworkName(networkName);

        NetworksCreateResponse response = await Client.Networks.CreateNetworkAsync(
            new NetworksCreateParameters { Name = uniqueNetworkName });

        Networks.Add(networkName, uniqueNetworkName);
        return response.ID;
    }

    private async Task RemoveNetworkIfUnused(string networkName)
    {
        string uniqueNetworkName = Networks[networkName];
        await Retry(async () =>
        {
            NetworkResponse? inspectResponse = (await Client.Networks.ListNetworksAsync())
                .FirstOrDefault(n => n.Name == uniqueNetworkName);

            if (inspectResponse != null && !inspectResponse.Containers.Any())
            {
                try
                {
                    await Client.Networks.DeleteNetworkAsync(inspectResponse.ID);
                }
                catch (DockerApiException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    _settings.Logger.Warning(
                        $"Cloud not remove network {inspectResponse.ID}. {ex.ResponseBody}");
                }
                catch (DockerNetworkNotFoundException)
                {
                    _settings.Logger.Information(
                        $"Network {inspectResponse.ID} has already been removed.");
                }
            }
        });
    }

    private Task<TResult> Retry<TResult>(Func<Task<TResult>> execute)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2), RetryAction)
            .ExecuteAsync(async () => await execute());
    }

    private Task Retry(Func<Task> execute)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(10, _ => TimeSpan.FromSeconds(5), RetryAction)
            .ExecuteAsync(async () => await execute());
    }

    private async Task RetryAction(Exception exception, TimeSpan t, int retryCount, Context c)
    {
        _settings.Logger.Warning($"Docker command failed {retryCount}. {exception.Message}");

        SystemInfoResponse? systemInfo = await Client.System.GetSystemInfoAsync();

        if (systemInfo is { DriverStatus: { Count: > 0 } })
        {
            _settings.Logger.Warning($"Driver status: {string.Join(", ", systemInfo.DriverStatus)}");
        }

        if (systemInfo is { SystemStatus: { Count: > 0 } })
        {
            _settings.Logger.Warning($"System status: {string.Join(", ", systemInfo.SystemStatus)}");
        }
    }

    public void Dispose()
    {
        Client?.Dispose();
        Instance?.Dispose();
    }
}
