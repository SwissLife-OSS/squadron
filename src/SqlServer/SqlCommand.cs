using System.Text;
using Docker.DotNet.Models;

namespace Squadron
{
    internal class SqlCommand : ICommand
    {
        private readonly StringBuilder _command = new StringBuilder();

        private SqlCommand(
            string command,
            ContainerResourceSettings settings)
        {
            _command.Append("/opt/mssql-tools18/bin/sqlcmd ");
            _command.Append($"-S localhost -U {settings.Username} -P {settings.Password} -C ");
            _command.Append(command);
        }

        internal static ContainerExecCreateParameters ExecuteFile(
            string inputFile,
            ContainerResourceSettings settings)
            => new SqlCommand($"-i {inputFile}", settings)
             .ToContainerExecCreateParameters();

        internal static ContainerExecCreateParameters ExecuteQuery(
            string query,
            ContainerResourceSettings settings)
            => new SqlCommand($"-q '{query}'", settings)
                .ToContainerExecCreateParameters();

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
