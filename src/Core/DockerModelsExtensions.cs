namespace Squadron;

internal static class DockerModelsExtensions
{
    /// <summary>
    /// Converts an ICommand to a string array for Testcontainers ExecAsync.
    /// </summary>
    internal static string[] ToCommandArray(this ICommand command)
    {
        return command.Command.Split(' ');
    }

    /// <summary>
    /// Converts an ICommand to a string array for Testcontainers ExecAsync.
    /// User parameter is ignored in Testcontainers - exec runs as container user.
    /// </summary>
    internal static string[] ToCommandArray(this ICommand command, string user)
    {
        // Note: Testcontainers ExecAsync doesn't support specifying a user
        // If user-specific execution is needed, consider using 'su' command
        return command.Command.Split(' ');
    }
}