using System.Text;

namespace Squadron;

internal class MongoImportCommand : ICommand
{
    private readonly StringBuilder _command = new StringBuilder();

    internal MongoImportCommand(
        string inputFile,
        string databaseName,
        string collectionName,
        string[] customArgs)
    {
        _command.Append("mongoimport ");
        _command.Append($"--db={databaseName} ");
        _command.Append($"--collection={collectionName} ");
        _command.Append($"--file={inputFile.Replace("\\", "/")}");

        if (customArgs.Length > 0)
        {
            _command.Append($" {string.Join(" ", customArgs)}");
        }
    }

    public string Command => _command.ToString();
}