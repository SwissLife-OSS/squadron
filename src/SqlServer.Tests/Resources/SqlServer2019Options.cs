using System;

namespace Squadron.Resources
{
    /// <summary>
    /// SqlServer 2019 options
    /// </summary>
    public class SqlServer2019Options : ContainerResourceOptions
    {
        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            var password = "_Qtp" + Guid.NewGuid().ToString("N");
            builder
                .Name("mssql")
                .Image("mcr.microsoft.com/mssql/server:2019-latest")
                .InternalPort(1433)
                .Username("sa")
                .Password(password)
                .WaitTimeout(60 * 5)
                .AddEnvironmentVariable("ACCEPT_EULA=Y")
                .AddEnvironmentVariable($"SA_PASSWORD={password}");
        }
    }
}
