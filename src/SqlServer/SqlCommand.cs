using System.Text;

namespace Squadron
{
    internal class SqlCommand : ICommand
    {
        private readonly StringBuilder _command = new StringBuilder();

        private SqlCommand(
            string command,
            IImageSettings settings)
        {
            _command.Append("/opt/mssql-tools/bin/sqlcmd ");
            _command.Append($"-S localhost -U {settings.Username} -P {settings.Password} ");
            _command.Append(command);
        }

        internal static SqlCommand ExecuteFile(
            string inputFile,
            IImageSettings settings)
            => new SqlCommand($"-i {inputFile}", settings);

        internal static SqlCommand ExecuteQuery(
            string query,
            IImageSettings settings)
            => new SqlCommand($"-q '{query}'", settings);

        public string Command => _command.ToString();
    }

    internal static class SqlHelpers
    {
        public static string Drop(string databaseName)
            => new StringBuilder()
                .AppendLine("USE [master];")
                .AppendLine("GO")
                .AppendLine($"DROP DATABASE '{databaseName}';")
                .ToString();
    }
}
