using System.Text;

namespace Squadron;

/// <summary>
/// PostgreSQL CreateDb command
/// </summary>
public class CreateDbCommand : ICommand
{
    private readonly StringBuilder _command = new StringBuilder();

    private CreateDbCommand(
        string dbname,
        ContainerResourceSettings settings)
    {
        _command.Append("createdb ");
        _command.Append($"-h localhost -U {settings.Username} ");
        _command.Append(dbname);
    }

    internal static string[] Execute(string name,
        ContainerResourceSettings settings)
        => new CreateDbCommand(name, settings)
            .ToCommandArray();

    /// <summary>
    /// Command
    /// </summary>
    public string Command => _command.ToString();
}