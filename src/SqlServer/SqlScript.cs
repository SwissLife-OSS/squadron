using System;
using System.IO;
using System.Threading.Tasks;

namespace Squadron
{
    internal static class SqlScript
    {
        internal static async Task DeployAndExecute(
            string sqlScript,
            IImageSettings settings)
        {
            FileInfo scriptFile = CreateSqlFile(sqlScript);
            CopyContext copyContext = new CopyContext(scriptFile.FullName, $"/tmp/{scriptFile.Name}");

            await Container.CopyTo(
                copyContext, settings);
            await Container.InvokeCommand(
                SqlCommand.ExecuteFile(copyContext.Destination, settings),
                settings);

            File.Delete(scriptFile.FullName);
        }

        private static FileInfo CreateSqlFile(string content)
        {
            var scriptFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".sql");
            File.WriteAllText(scriptFile, content);
            return new FileInfo(scriptFile);
        }
    }
}
