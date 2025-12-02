
using System;
using Docker.DotNet.Models;

namespace Squadron;

/// <summary>
/// Execute a chmod command as root
/// </summary>
public static class ChmodCommand
{
    /// <summary>
    /// Change file mode
    /// </summary>
    /// <param name="pathInContainer">Path within container</param>
    /// <param name="owner"><see cref="Permission"/> for owner</param>
    /// <param name="group"><see cref="Permission"/> for group</param>
    /// <param name="public"><see cref="Permission"/> for public</param>
    /// <param name="recursive">Apply recursive</param>
    public static ContainerExecCreateParameters Set(
        string pathInContainer,
        Permission owner = Permission.None,
        Permission group = Permission.None,
        Permission @public = Permission.None,
        bool recursive = false)
    {
        var cmd = $"chmod {(recursive ? "-R " : "")}{owner:d}{group:d}{@public:d} {pathInContainer}";

        return new ContainerExecCreateParameters
        {
            AttachStderr = true,
            AttachStdin = false,
            AttachStdout = true,
            Cmd = cmd.Split(' '),
            Privileged = true,
            User = "root"
        };
    }

    public static ContainerExecCreateParameters ReadOnly(string pathInContainer, bool recursive = false)
        => Set(pathInContainer, Permission.Read, Permission.Read, Permission.Read, recursive);

    public static ContainerExecCreateParameters FullAccess(string pathInContainer, bool recursive = false)
        => Set(pathInContainer, Permission.FullAccess, Permission.FullAccess, Permission.FullAccess, recursive);

    public static ContainerExecCreateParameters Execute(string pathInContainer, bool recursive = false)
        => Set(pathInContainer, Permission.Execute, Permission.Execute, Permission.Execute, recursive);

    public static ContainerExecCreateParameters ReadWrite(string pathInContainer, bool recursive = false)
        => Set(pathInContainer, Permission.ReadWrite, Permission.ReadWrite, Permission.ReadWrite, recursive);

    [Flags]
    public enum Permission
    {
        None = 0b000,
        Read = 0b100,
        Write = 0b010,
        Execute = 0b001,
        FullAccess = Read | Write | Execute,
        ReadWrite = Read | Write
    }
}