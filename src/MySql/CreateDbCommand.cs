using Docker.DotNet.Models;

namespace Squadron;

internal class CreateDbCommand : SqlCommandBase, ICommand
{
    public ContainerExecCreateParameters Parameters { get; }

    private CreateDbCommand(
        string dbname,
        string? grant,
        ContainerResourceSettings settings)
    {
        var grantAccess = grant ?? "ALL";
        Parameters = GetContainerExecParameters(
            $@"CREATE DATABASE {dbname};
                   CREATE ROLE developer_{dbname};
                   GRANT {grantAccess}
                   ON {dbname}.*
                   TO '{settings.Username}';",
            settings);
    }

    internal static ContainerExecCreateParameters Execute(
        string dbName,
        string? grant,
        ContainerResourceSettings settings)
        => new CreateDbCommand(dbName, grant, settings).Parameters;

    public string Command => string.Join(" ", Parameters.Cmd);
}