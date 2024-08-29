using System;

namespace Squadron
{
    /// <summary>
    /// Default SqlServer resource options
    /// </summary>
    public class SqlServerDefaultOptions : ContainerResourceOptions
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
                .WaitTimeout(60)
                .Username("sa")
                .Password(password)
                .AddEnvironmentVariable("ACCEPT_EULA=Y")
                .AddEnvironmentVariable($"SA_PASSWORD={password}")
                .PreferLocalImage();
        }
    }
}
