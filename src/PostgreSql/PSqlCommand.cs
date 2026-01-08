using System.Text;

namespace Squadron;

internal class PSqlCommand : ICommand
{
    private readonly StringBuilder _command = new StringBuilder();

    private PSqlCommand(
        string command,
        ContainerResourceSettings settings)
    {
        _command.Append("psql ");
        _command.Append($"-h localhost -U {settings.Username} ");
        _command.Append(command);
    }

    internal static string[] ExecuteFile(
        string inputFile,
        string dbName,
        ContainerResourceSettings settings)
        => new PSqlCommand($"{dbName} -f {inputFile}", settings)
            .ToCommandArray();

    public string Command => _command.ToString();
}