using System.Collections.Generic;
using Docker.DotNet.Models;

namespace Squadron
{
    internal static class DockerModelsExtensions
    {
        internal static ContainerExecCreateParameters ToContainerExecCreateParameters(
            this ICommand command)
        {
            return new ContainerExecCreateParameters
            {
                AttachStderr = true,
                AttachStdin = false,
                AttachStdout = true,
                Cmd = command.Command.Split(' '),
                Detach = false,
                Tty = false
            };
        }

        public static CreateContainerParameters ToCreateContainerParameters(
            this IImageSettings imageSettings)
        {
            return new CreateContainerParameters
            {
                Name = imageSettings.Name,
                Image = imageSettings.Image,
                AttachStdout = true,
                AttachStderr = true,
                AttachStdin = false,
                Tty = false,
                HostConfig = new HostConfig
                {
                    PublishAllPorts = true
                },
                Env = imageSettings.EnvironmentVariable
            };
        }
    }
}
