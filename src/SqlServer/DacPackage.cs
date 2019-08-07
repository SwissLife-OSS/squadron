#if NET46
using System.IO;
using Microsoft.SqlServer.Dac;

namespace Squadron
{
    internal static class DacPackage
    {
        internal static void Deploy(
            Stream source,
            string databaseConnection,
            string databaseName,
            IImageSettings settings)
        {
            using (var dacPackage = Microsoft.SqlServer.Dac.DacPackage.Load(source))
            {
                var deployService = new DacServices(databaseConnection);
                var deployScriptRaw = deployService.GenerateDeployScript(dacPackage, databaseName, DacpacOptions.Deploy);
                var deployScript = new DeployScript(deployScriptRaw);

                deployScript.SetVariable("DefaultDataPath", "/tmp/");
                deployScript.SetVariable("DefaultLogPath", "/tmp/");

                var sqlScript = deployScript.Generate();
                SqlScript.DeployAndExecute(sqlScript, settings);
            }
        }
    }
}
#endif
