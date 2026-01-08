namespace Squadron;

internal static class DockerModelsExtensions
{
    /// <summary>
    /// Converts an ICommand to a command array for container execution.
    /// </summary>
    internal static string[] ToCommandArray(this ICommand command)
    {
        return command.Command.Split(' ');
    }
}