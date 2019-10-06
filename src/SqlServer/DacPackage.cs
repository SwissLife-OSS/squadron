#if NET46
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.SqlServer.Dac;

namespace Squadron
{
    internal static class DacPackage
    {
        internal async static Task DeployAsync(
            this IDockerContainerManager manager,
            ContainerResourceSettings settings,
            Stream source,
            string databaseConnection,
            string databaseName)
        {
            using (var dacPackage = Microsoft.SqlServer.Dac.DacPackage.Load(source))
            {
                var deployService = new DacServices(databaseConnection);
                var deployScriptRaw = deployService.GenerateDeployScript(dacPackage, databaseName, DacpacOptions.Deploy);
                var deployScript = new DeployScript(deployScriptRaw);

                deployScript.SetVariable("DefaultDataPath", "/tmp/");
                deployScript.SetVariable("DefaultLogPath", "/tmp/");

                var sqlScript = deployScript.Generate();

                FileInfo scriptFile = CreateSqlFile(sqlScript);
                var copyContext = new CopyContext(scriptFile.FullName, $"/tmp/{scriptFile.Name}");

                await manager.CopyToContainer(copyContext);
                await manager.InvokeCommandAsync(SqlCommand.ExecuteFile(copyContext.Destination, settings));
                File.Delete(scriptFile.FullName);
            }
        }

        private static FileInfo CreateSqlFile(string content)
        {
            var scriptFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".sql");
            File.WriteAllText(scriptFile, content);
            return new FileInfo(scriptFile);
        }
    }
}
#endif
