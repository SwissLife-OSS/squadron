using System.Text;
using Docker.DotNet.Models;


namespace Squadron
{
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

        internal static ContainerExecCreateParameters ExecuteFile(
            string inputFile,
            string dbName,
            ContainerResourceSettings settings)
            => new PSqlCommand($"{dbName} -f {inputFile}", settings)
             .ToContainerExecCreateParameters();

        public string Command => _command.ToString();
    }
}
