using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Version = System.Version;

namespace Squadron
{
    public class DockerContainerManager : IDockerContainerManager
    {
        public ContainerInstance Instance { get; } = new ContainerInstance();

        private readonly ContainerResourceSettings _settings;
        private readonly AuthConfig _authConfig = new AuthConfig();

        private readonly DockerClient _client =
            new DockerClientConfiguration(
                    LocalDockerUri(),
                    null,
                    TimeSpan.FromMinutes(5))
                .CreateClient(Version.Parse("1.25"));

        private static Uri LocalDockerUri()
        {
#if NET46
            return new Uri("npipe://./pipe/docker_engine");
#else
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ?
                new Uri("npipe://./pipe/docker_engine") :
                new Uri("unix:/var/run/docker.sock");
#endif
        }

        public DockerContainerManager(ContainerResourceSettings settings)
        {
            _settings = settings;
        }


        public async Task CreateAndStartContainerAsync()
        {
            await PullImageAsync();
            await CreateContainerAsync();
            await StartContainerAsync();
            await ResolveHostAddressAsync();

            if (!Instance.IsRunning)
            {
                var logs = await ConsumeLogsAsync(TimeSpan.FromSeconds(5));
                throw new ContainerException(
                    $"Container exited with following logs: \r\n {logs}");
            }

            ////settings.Logs = await ConsumeLogs(settings, TimeSpan.FromSeconds(10));
            //var success = await ResolveContainerAddress(settings) &&
            //    await ResolveHostPort(settings);

            //if (!success)
            //{

            //}
        }

        private async Task StartContainerAsync()
        {
            var containerStartParameters = new ContainerStartParameters();

            bool started = await _client.Containers.StartContainerAsync(
                Instance.Id,
                containerStartParameters);

            if (!started)
            {
                throw new ContainerException(
                    "Docker container creation/startup failed.");
            }
        }

        private async Task CreateContainerAsync()
        {
            var startParams = new CreateContainerParameters
            {
                Name = _settings.UniqueContainerName,
                Image = _settings.ImageFullname,
                AttachStdout = true,
                AttachStderr = true,
                AttachStdin = false,
                Tty = false,
                HostConfig = new HostConfig
                {
                    PublishAllPorts = true
                },
                Env = _settings.EnvironmentVariables
            };

            CreateContainerResponse response = await _client
                .Containers
                .CreateContainerAsync(startParams);

            if (string.IsNullOrEmpty(response.ID))
            {
                throw new ContainerException(
                    "Could not create the container");
            }
            Instance.Id = response.ID;
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

            await _client.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = _settings.ImageFullname },
                _authConfig,
                new Progress<JSONMessage>(Handler));
        }

        public async Task<bool> StopContainerAsync()
        {
            var stopOptions = new ContainerStopParameters
            {
                WaitBeforeKillSeconds = 5
            };

            bool stopped = await _client.Containers
                .StopContainerAsync(Instance.Id, stopOptions, default);

            return stopped;
        }

        public async Task RemoveContainerAsync()
        {
            var removeOptions = new ContainerRemoveParameters
            {
                Force = true,
                RemoveVolumes = true
            };

            await _client.Containers
                .RemoveContainerAsync(Instance.Id, removeOptions);
        }

        private async Task ResolveHostAddressAsync()
        {
            ContainerInspectResponse inspectResponse = await _client
                .Containers
                .InspectContainerAsync(Instance.Id);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Instance.Address = "localhost";
                string containerPort = $"{_settings.InternalPort}/tcp";
                if (!inspectResponse.NetworkSettings.Ports.ContainsKey(containerPort))
                {
                    throw new Exception($"Failed to resolve host port for {containerPort}");
                }

                PortBinding binding = inspectResponse
                    .NetworkSettings
                    .Ports[containerPort]
                    .FirstOrDefault();

                if (binding == null || string.IsNullOrEmpty(binding.HostPort))
                {
                    throw new Exception($"The resolved port binding is empty");
                }

                Instance.HostPort = int.Parse(binding.HostPort);
            }
            else
            {
                Instance.Address = inspectResponse.NetworkSettings.IPAddress;
                Instance.HostPort = _settings.InternalPort;
            }
            Instance.IsRunning = inspectResponse.State.Running;
        }

        public async Task CopyToContainer(CopyContext context)
        {
            using (var archiver = new TarArchiver(context.Source))
            {
                await _client.Containers.ExtractArchiveToContainerAsync(
                    Instance.Id,
                    new ContainerPathStatParameters
                    {
                        AllowOverwriteDirWithFile = true,
                        Path = context.DestinationFolder.Replace("\\", "/")
                    }, archiver.Stream);
            }
        }

        public async Task InvokeCommandAsync(
                ContainerExecCreateParameters parameters)
        {
            ContainerExecCreateResponse response = await _client.Containers
                .ExecCreateContainerAsync(
                    Instance.Id,
                    parameters);

            if (!string.IsNullOrEmpty(response.ID))
            {
                using (MultiplexedStream stream = await _client.Containers
                    .StartAndAttachContainerExecAsync(
                        response.ID, false))
                {
                    (string stdout, string stderr) output = await stream
                        .ReadOutputToEndAsync(CancellationToken.None);

                    if (!string.IsNullOrEmpty(output.stderr) && output.stderr.Contains("error"))
                    {
                        var error = new StringBuilder();
                        error.AppendLine($"Error when invoking command \"{string.Join(" ", parameters.Cmd)}\"");
                        error.AppendLine(output.stderr);

                        throw new ContainerException(error.ToString());
                    }
                }
            }
        }

        public async Task<string> ConsumeLogsAsync(TimeSpan timeout)
        {
            var containerStatsParameters = new ContainerLogsParameters
            {
                Follow = true,
                ShowStderr = true,
                ShowStdout = true
            };

            Stream logStream = await _client
                .Containers
                .GetContainerLogsAsync(
                    Instance.Id,
                    containerStatsParameters,
                    default);

            var logs = await ReadAsync(logStream, timeout);
            Instance.Logs.Add(logs);
            Trace.TraceInformation(logs);
            return logs;
        }

        private async Task<string> ReadAsync(
            Stream logStream,
            TimeSpan timeout)
        {
            var result = new StringBuilder();
            var timeoutTask = Task.Delay(timeout);
            using (logStream)
            {
                const int size = 256;
                byte[] buffer = new byte[size];

                while (true)
                {
                    Task<int> readTask = logStream.ReadAsync(
                        buffer, 0, size);

                    if (await Task.WhenAny(readTask, timeoutTask) == timeoutTask)
                    {
                        logStream.Close();
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

                    string chunk = new string(
                        chunkChars, 0, consumed);

                    result.Append(chunk);
                }
            }
            return result.ToString();
        }
    }
}
