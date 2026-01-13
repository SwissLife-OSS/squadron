using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;

namespace Squadron;

/// <summary>
/// Manager to work with docker containers
/// </summary>
/// <seealso cref="Squadron.IDockerContainerManager" />
public class TestcontainersDockerManager : IDockerContainerManager
{
    private static readonly IDictionary<string, INetwork> Networks = new Dictionary<string, INetwork>();
    private static readonly SemaphoreSlim SyncNetworks = new(1, 1);

    private readonly ContainerResourceSettings _settings;
    private readonly VariableResolver _variableResolver;

    private IContainer? _container;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainersDockerManager"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public TestcontainersDockerManager(ContainerResourceSettings settings)
    {
        _settings = settings;
        _variableResolver = new VariableResolver(_settings.Variables);
    }

    public ContainerInstance Instance { get; } = new ContainerInstance();

    public IContainer Container => _container
        ?? throw new InvalidOperationException("Container has not been created yet.");

    /// <inheritdoc/>
    public async Task CreateAndStartContainerAsync()
    {
        ResolveAndReplaceVariables();

        var builder = new ContainerBuilder()
            .WithName(_settings.UniqueContainerName)
            .WithImage(_settings.ImageFullname)
            .WithCleanUp(true)
            .WithAutoRemove(false);

        // Configure port bindings
        builder = ConfigurePortBindings(builder);

        // Configure environment variables
        foreach (var envVar in _settings.EnvironmentVariables)
        {
            var parts = envVar.Split('=', 2);
            if (parts.Length == 2)
            {
                builder = builder.WithEnvironment(parts[0], parts[1]);
            }
        }

        // Configure volumes
        foreach (var volume in _settings.Volumes)
        {
            var parts = volume.Split(':');
            if (parts.Length >= 2)
            {
                builder = builder.WithBindMount(parts[0], parts[1]);
            }
        }

        // Configure command
        if (_settings.Cmd?.Count > 0)
        {
            builder = builder.WithCommand(_settings.Cmd.ToArray());
        }

        // Configure memory limit
        if (_settings.Memory > 0)
        {
            builder = builder.WithCreateParameterModifier(param =>
            {
                param.HostConfig.Memory = _settings.Memory;
            });
        }

        // Configure files to copy
        foreach (var copyContext in _settings.FilesToCopy)
        {
            var fileBytes = await File.ReadAllBytesAsync(copyContext.Source);
            builder = builder.WithResourceMapping(fileBytes, copyContext.Destination);
        }

        // Configure networks
        foreach (var networkName in _settings.Networks)
        {
            var network = await GetOrCreateNetworkAsync(networkName);
            builder = builder.WithNetwork(network)
                .WithNetworkAliases(_settings.UniqueContainerName);
        }

        // Build and start container
        _container = builder.Build();

        try
        {
            _settings.Logger.Verbose("Starting container");

            await _container.StartAsync();
            _settings.Logger.Information("Container started");

            // Populate instance details
            await PopulateInstanceDetailsAsync();
        }
        catch (Exception ex)
        {
            _settings.Logger.Error("Container start failed", ex);
            throw new ContainerException(
                $"Error starting container: {_settings.UniqueContainerName}", ex);
        }
    }

    private ContainerBuilder ConfigurePortBindings(ContainerBuilder builder)
    {
        var allPorts = new List<ContainerPortMapping>
        {
            new()
            {
                InternalPort = _settings.InternalPort,
                ExternalPort = _settings.ExternalPort,
                HostIp = _settings.HostIp
            }
        };
        allPorts.AddRange(_settings.AdditionalPortMappings);

        foreach (var portMapping in allPorts)
        {
            if (portMapping.ExternalPort != 0)
            {
                // Static port mapping: hostPort, containerPort
                builder = builder.WithPortBinding(portMapping.ExternalPort, portMapping.InternalPort);
            }
            else
            {
                // Dynamic port mapping (let Docker choose): containerPort, assignRandomHostPort
                builder = builder.WithPortBinding(portMapping.InternalPort, true);
            }
        }

        return builder;
    }

    private async Task PopulateInstanceDetailsAsync()
    {
        if (_container == null)
        {
            throw new ContainerException("Container is not initialized");
        }

        Instance.Id = _container.Id;
        Instance.Name = _container.Name.TrimStart('/');
        Instance.IsRunning = _container.State == TestcontainersStates.Running;

        // Get the mapped port for the main internal port
        try
        {
            Instance.HostPort = _container.GetMappedPublicPort(_settings.InternalPort);
            Instance.Address = _container.Hostname;

            // Resolve additional ports
            foreach (var portMapping in _settings.AdditionalPortMappings)
            {
                var mappedPort = _container.GetMappedPublicPort(portMapping.InternalPort);
                Instance.AdditionalPorts.Add(new ContainerPortMapping
                {
                    InternalPort = portMapping.InternalPort,
                    ExternalPort = mappedPort,
                    HostIp = portMapping.HostIp
                });
            }
        }
        catch (Exception ex)
        {
            _settings.Logger.Error("Failed to resolve port bindings", ex);
            throw new ContainerException("Failed to resolve host port bindings", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<bool> StopContainerAsync()
    {
        if (_container == null)
        {
            return false;
        }

        try
        {
            await _container.StopAsync();
            _settings.Logger.Information("Container stopped");
            return true;
        }
        catch (Exception ex)
        {
            _settings.Logger.Error("Failed to stop container", ex);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task RemoveContainerAsync()
    {
        if (_container == null)
        {
            return;
        }

        try
        {
            await _container.DisposeAsync();

            // Clean up networks
            foreach (var networkName in _settings.Networks)
            {
                await RemoveNetworkIfUnused(networkName);
            }
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
        if (_container == null)
        {
            throw new ContainerException("Container is not initialized");
        }

        try
        {
            var fileBytes = await File.ReadAllBytesAsync(context.Source);
            const UnixFileModes fileMode = UnixFileModes.UserRead | UnixFileModes.UserWrite | 
                                           UnixFileModes.GroupRead | UnixFileModes.OtherRead;
            await _container.CopyAsync(fileBytes, context.Destination, (uint)fileMode);
        }
        catch (Exception ex)
        {
            throw new ContainerException(
                $"Error copying file to container: {context.Source} -> {context.Destination}", ex);
        }
    }

    /// <inheritdoc/>
    public async Task<string?> InvokeCommandAsync(string[] command, int retryCount = 0, int retryDelayMs = 1000)
    {
        if (_container == null)
        {
            return null;
        }

        Exception? lastException = null;
        int attempts = retryCount + 1;

        for (int i = 0; i < attempts; i++)
        {
            try
            {
                var result = await _container.ExecAsync(command);

                if (result.ExitCode != 0)
                {
                    var error = new StringBuilder();
                    error.AppendLine($"Error when invoking command \"{string.Join(" ", command)}\"");
                    error.AppendLine($"Exit code: {result.ExitCode}");
                    if (!string.IsNullOrWhiteSpace(result.Stderr))
                    {
                        error.AppendLine($"Stderr: {result.Stderr}");
                    }
                    if (!string.IsNullOrWhiteSpace(result.Stdout))
                    {
                        error.AppendLine($"Stdout: {result.Stdout}");
                    }

                    throw new ContainerException(error.ToString());
                }

                return result.Stdout;
            }
            catch (ContainerException ex)
            {
                lastException = ex;
                if (i < attempts - 1)
                {
                    await Task.Delay(retryDelayMs);
                }
            }
            catch (Exception ex)
            {
                lastException = new ContainerException(
                    $"Error invoking command: {string.Join(" ", command)}", ex);
                if (i < attempts - 1)
                {
                    await Task.Delay(retryDelayMs);
                }
            }
        }

        throw lastException!;
    }

    /// <inheritdoc/>
    public async Task<string> ConsumeLogsAsync(TimeSpan timeout)
    {
        if (_container == null)
        {
            return "No container";
        }

        try
        {
            using var cts = new CancellationTokenSource(timeout);
            var (stdout, stderr) = await _container.GetLogsAsync(ct: cts.Token);

            var logs = new StringBuilder();
            if (!string.IsNullOrEmpty(stdout))
            {
                logs.AppendLine(stdout);
            }
            if (!string.IsNullOrEmpty(stderr))
            {
                logs.AppendLine(stderr);
            }

            var logString = logs.ToString();
            Instance.Logs.Add(logString);
            _settings.Logger.ContainerLogs(logString);
            return logString;
        }
        catch (Exception ex)
        {
            _settings.Logger.Error("Failed to get logs", ex);
            return $"Error getting logs: {ex.Message}";
        }
    }

    private void ResolveAndReplaceVariables()
    {
        ResolveAdditionalPortsVariables();
        ReplaceVariablesInEnvironmentalVariables();
    }

    private void ReplaceVariablesInEnvironmentalVariables()
    {
        foreach (var variable in _settings.Variables)
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
        foreach (var additionalPort in _settings.AdditionalPortMappings)
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

    private async Task<INetwork> GetOrCreateNetworkAsync(string networkName)
    {
        await SyncNetworks.WaitAsync();

        try
        {
            if (Networks.TryGetValue(networkName, out var existingNetwork))
            {
                return existingNetwork;
            }

            var uniqueNetworkName = UniqueNameGenerator.CreateNetworkName(networkName);

            var network = new NetworkBuilder()
                .WithName(uniqueNetworkName)
                .WithCleanUp(true)
                .Build();

            await network.CreateAsync();
            Networks.Add(networkName, network);

            return network;
        }
        finally
        {
            SyncNetworks.Release();
        }
    }

    private async Task RemoveNetworkIfUnused(string networkName)
    {
        await SyncNetworks.WaitAsync();

        try
        {
            if (Networks.TryGetValue(networkName, out var network))
            {
                try
                {
                    await network.DisposeAsync();
                    Networks.Remove(networkName);
                }
                catch (Exception ex)
                {
                    _settings.Logger.Warning($"Could not remove network {networkName}. {ex.Message}");
                }
            }
        }
        finally
        {
            SyncNetworks.Release();
        }
    }

    public void Dispose()
    {
        _container?.DisposeAsync().AsTask().GetAwaiter().GetResult();
        Instance?.Dispose();
    }
}
