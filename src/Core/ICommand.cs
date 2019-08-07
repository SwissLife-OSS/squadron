namespace Squadron
{
    /// <summary>
    /// Represents a command that targets a container.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the full command which is executed in bash mode inside container
        /// </summary>
        /// <returns></returns>
        string Command { get; }
    }
}
