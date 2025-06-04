using System.Collections.Generic;
using System.Text;
using Docker.DotNet.Models;


namespace Squadron;

internal class SqlCommand: SqlCommandBase, ICommand
{
    public ContainerExecCreateParameters Parameters { get; }

    private SqlCommand(
        string command,
        ContainerResourceSettings settings)
    {
        Parameters = GetContainerExecParameters(command, settings);
    }

    internal static ContainerExecCreateParameters Execute(
        string inputFile,
        string dbName,
        ContainerResourceSettings settings)
        => new SqlCommand($"Use {dbName}; {inputFile}", settings).Parameters;

    public string Command => string.Join(" ", Parameters.Cmd);
}