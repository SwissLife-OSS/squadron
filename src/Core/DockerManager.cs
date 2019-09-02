using System;
using System.Collections.Generic;
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
    internal static class DockerManager
    {
        private static readonly AuthConfig _authConfig =
            new AuthConfig();

        private static readonly DockerClient _client =
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

        public static async Task<bool> StopAndRemoveContainer(string containerName)
        {
            try
            {
                ContainerListResponse container = await GetContainer(containerName);
                if (container == null)
                {
                    return false;
                }

                bool success = await StopContainer(container);
                if (success)
                {
                    await RemoveContainer(container);
                }

                return success;
            }
            catch
            {
                // Should not fail tests if cannot stop container.
                return false;
            }
        }

        public static async Task CreateAndStartContainer(
            IImageSettings settings)
        {
            await PullImage(settings);
            await CreateContainer(settings);

            bool startSuccessful = await StartContainer(settings.ContainerId);
            if (!startSuccessful)
            {
                throw new ContainerException(
                    "Docker container creation/startup failed.");
            }

            var logs = await ConsumeLogs(settings, TimeSpan.FromSeconds(10));
            var success = await ResolveContainerAddress(settings) &&
                await ResolveHostPort(settings);

            if (!success)
            {
                throw new ContainerException(
                    $"Container exited with following logs: \r\n {logs}");
            }
        }

        private static async Task PullImage(IImageSettings settings)
        {
            void Handler(JSONMessage message)
            {
                if (!string.IsNullOrEmpty(message.ErrorMessage))
                {
                    throw new ContainerException(
                        $"Could not pull the image: {settings.Image}. " +
                        $"Error: {message.ErrorMessage}");
                }
            }

            await _client.Images.CreateImageAsync(
                new ImagesCreateParameters { FromImage = settings.Image },
                _authConfig,
                new Progress<JSONMessage>(Handler));
        }

        private static async Task<string> ConsumeLogs(
            IImageSettings settings, TimeSpan timeout)
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
                    settings.ContainerId,
                    containerStatsParameters,
                    CancellationToken.None);

            Task<string> logsTask = ReadAsync(logStream);
            Task timeoutTask = Task.Delay(timeout);
            if (await Task.WhenAny(logsTask, timeoutTask) == logsTask)
            {
                return await logsTask;
            }

            return $"Container exited, please check logs for container {settings.ContainerId}";
        }

        private static async Task<bool> ResolveContainerAddress(IImageSettings settings)
        {
            ContainerInspectResponse inspectResponse = await _client
                .Containers
                .InspectContainerAsync(settings.ContainerId);

            settings.ContainerAddress = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ?
                "localhost" :
                inspectResponse.NetworkSettings.IPAddress;

            return inspectResponse.State.Running;
        }

        private static async Task<bool> ResolveHostPort(IImageSettings settings)
        {
            ContainerInspectResponse inspectResponse = await _client
                .Containers
                .InspectContainerAsync(settings.ContainerId);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                string containerPort = $"{settings.ContainerPort}/tcp";
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

                settings.HostPort = long.Parse(binding.HostPort);
            }
            else
            {
                settings.HostPort = settings.ContainerPort;
            }

            return inspectResponse.State.Running;
        }

        private static async Task<IList<ContainerListResponse>> GetAllContainers()
        {
            var listOption = new ContainersListParameters { All = true };
            IList<ContainerListResponse> containers =
                await _client.Containers.ListContainersAsync(listOption);

            return containers;
        }

        private static async Task<ContainerListResponse> GetContainer(string containerName)
        {
            IList<ContainerListResponse> containers = await GetAllContainers();

            ContainerListResponse container = containers
                .SingleOrDefault(c => c.Names.Select(n => n.Trim('/'))
                    .Any(n => n == containerName));

            return container;
        }

        private static async Task<bool> StopContainer(ContainerListResponse container)
        {
            var stopOptions = new ContainerStopParameters
            {
                WaitBeforeKillSeconds = 5
            };

            bool stopped = await _client.Containers
                .StopContainerAsync(container.ID, stopOptions, CancellationToken.None);

            return stopped;
        }

        private static async Task RemoveContainer(ContainerListResponse container)
        {
            var removeOptions = new ContainerRemoveParameters
            {
                Force = true,
                RemoveVolumes = true
            };

            await _client.Containers
                .RemoveContainerAsync(container.ID, removeOptions);
        }

        private static async Task CreateContainer(
            IImageSettings settings)
        {
            CreateContainerResponse response = await _client
                .Containers
                .CreateContainerAsync(settings.ToCreateContainerParameters());

            if (string.IsNullOrEmpty(response.ID))
            {
                throw new ContainerException(
                    "Could not create the container");
            }

            settings.ContainerId = response.ID;
        }

        private static async Task<bool> StartContainer(string containerId)
        {
            var containerStartParameters = new ContainerStartParameters();

            return await _client.Containers.StartContainerAsync(
                containerId,
                containerStartParameters);
        }

        public static async Task CopyToContainer(CopyContext context, IImageSettings settings)
        {
            using (var archiver = new TarArchiver(context.Source))
            {
                await _client.Containers.ExtractArchiveToContainerAsync(
                    settings.ContainerId,
                    new ContainerPathStatParameters
                    {
                        AllowOverwriteDirWithFile = true,
                        Path = context.DestinationFolder.Replace("\\", "/")
                    }, archiver.Stream);
            }
        }

        public static async Task InvokeCommand(
            ContainerExecCreateParameters parameters,
            IImageSettings settings)
        {
            ContainerExecCreateResponse response = await _client.Containers
                .ExecCreateContainerAsync(
                    settings.ContainerId,
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

        private static async Task<string> ReadAsync(Stream logStream)
        {
            var result = new StringBuilder();
            using (logStream)
            {
                const int size = 256;
                byte[] buffer = new byte[size];

                while (true)
                {
                    int read = await logStream.ReadAsync(
                        buffer, 0, size);

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
