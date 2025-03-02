namespace Squadron;

public class AzureServiceBusDefaultOptions<TConfig> : ComposeResourceOptions
    where TConfig : AzureServiceBusConfig, new()
{
    public override void Configure(ComposeResourceBuilder builder)
    {
        builder.AddContainer<SqlServerDefaultOptions>(
            AzureServiceBusConstants.SqlServerResourceName);

        builder
            .AddContainer<AzureServiceBusEmulatorDefaultOptions<TConfig>>(
                AzureServiceBusConstants.AzureServiceBusEmulatorResourceName)
            .AddLink(AzureServiceBusConstants.SqlServerResourceName,
                new EnvironmentVariableMapping("SQL_SERVER", "#NAME#"),
                new EnvironmentVariableMapping("MSSQL_SA_PASSWORD", "#MSSQL_SA_PASSWORD#"));
    }
}