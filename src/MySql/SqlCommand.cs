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
            _command.Append($"mysql -u root -p{settings.Password} -e \"{command}\";");
        }

        internal static ContainerExecCreateParameters ExecuteFile(
            string inputFile,
            string dbName,
            ContainerResourceSettings settings)
            => new SqlCommand($"Use {dbName}; {inputFile}", settings)
             .ToContainerExecCreateParameters();

        public string Command => _command.ToString();
    }
}
