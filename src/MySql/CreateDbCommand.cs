namespace Squadron;

internal class CreateDbCommand : SqlCommandBase, ICommand
{
    private readonly string[] _commandArray;

    private CreateDbCommand(
        string dbname,
        string? grant,
        ContainerResourceSettings settings)
    {
        var grantAccess = grant ?? "ALL";
        _commandArray = GetCommandArray(
            $@"CREATE DATABASE {dbname};
                   CREATE ROLE developer_{dbname};
                   GRANT {grantAccess}
                   ON {dbname}.*
                   TO '{settings.Username}';",
            settings);
    }

    internal static string[] Execute(
        string dbName,
        string? grant,
        ContainerResourceSettings settings)
        => new CreateDbCommand(dbName, grant, settings)._commandArray;

    public string Command => string.Join(" ", _commandArray);
}