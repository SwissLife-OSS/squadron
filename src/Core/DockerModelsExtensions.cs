using System.Collections.Generic;
using Docker.DotNet.Models;

namespace Squadron;

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
        
    internal static ContainerExecCreateParameters ToContainerExecCreateParameters(
        this ICommand command, string user)
    {
        return new ContainerExecCreateParameters
        {
            User = user,
            AttachStderr = true,
            AttachStdin = false,
            AttachStdout = true,
            Cmd = command.Command.Split(' '),
            Detach = false,
            Tty = false
        };
    }
}