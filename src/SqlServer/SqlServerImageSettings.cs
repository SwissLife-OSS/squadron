using System;
using System.Collections.Generic;

namespace Squadron
{
    public class SqlServerImageSettings : IImageSettings
    {
        public SqlServerImageSettings()
        {
            EnvironmentVariable =
                new List<string>
                {
                    "ACCEPT_EULA=Y",
                    $"SA_PASSWORD={Password}"
                };
        }

        public string Name { get; } = ContainerName.Create();
        public string Image { get; } = "microsoft/mssql-server-linux:latest";
        public long DefaultPort { get; } = 1433;
        public string ContainerId { get; set; }
        public string ContainerIp { get; set; }
        public string Username { get; } = "sa";
        public string Password { get; } = "_Qtp" + Guid.NewGuid().ToString("N");
        public List<string> EnvironmentVariable { get; }
    }
}
