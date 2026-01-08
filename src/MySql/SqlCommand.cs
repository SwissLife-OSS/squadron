namespace Squadron;

internal class SqlCommand : SqlCommandBase, ICommand
{
    private readonly string[] _commandArray;

    private SqlCommand(
        string command,
        ContainerResourceSettings settings)
    {
        _commandArray = GetCommandArray(command, settings);
    }

    internal static string[] Execute(
        string inputFile,
        string dbName,
        ContainerResourceSettings settings)
        => new SqlCommand($"Use {dbName}; {inputFile}", settings)._commandArray;

    public string Command => string.Join(" ", _commandArray);
}