using System;

namespace Squadron
{
    public class SqlServerDefaultOptions : ContainerResourceOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            var password = "_Qtp" + Guid.NewGuid().ToString("N");
            builder
                .Name("mssql")
                .Image("microsoft/mssql-server-linux:latest")
                .InternalPort(1433)
                .Username("sa")
                .Password(password)
                .AddEnvironmentVariable("ACCEPT_EULA=Y")
                .AddEnvironmentVariable($"SA_PASSWORD={password}");
        }
    }
}
