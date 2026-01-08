
using System;

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
    public static string[] Set(
        string pathInContainer,
        Permission owner = Permission.None,
        Permission group = Permission.None,
        Permission @public = Permission.None,
        bool recursive = false)
    {
        var cmd = $"chmod {(recursive ? "-R " : "")}{owner:d}{group:d}{@public:d} {pathInContainer}";
        return cmd.Split(' ');
    }

    public static string[] ReadOnly(string pathInContainer, bool recursive = false)
        => Set(pathInContainer, Permission.Read, Permission.Read, Permission.Read, recursive);

    public static string[] FullAccess(string pathInContainer, bool recursive = false)
        => Set(pathInContainer, Permission.FullAccess, Permission.FullAccess, Permission.FullAccess, recursive);

    public static string[] Execute(string pathInContainer, bool recursive = false)
        => Set(pathInContainer, Permission.Execute, Permission.Execute, Permission.Execute, recursive);

    public static string[] ReadWrite(string pathInContainer, bool recursive = false)
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