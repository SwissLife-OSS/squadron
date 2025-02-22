namespace Squadron;

public class AzureServiceBusDefaultOptions : ComposeResourceOptions
{
    internal static string SqlServerResourceName { get; } = "asb_sql_server";
    internal static string AzureServiceBusEmulatorResourceName { get; } = "asb_emulator";
    
    public override void Configure(ComposeResourceBuilder builder)
    {
        builder
            .AddContainer<SqlServerDefaultOptions>(SqlServerResourceName);
            
        builder
            .AddContainer<AzureServiceBusEmulatorDefaultOptions>(AzureServiceBusEmulatorResourceName)
            .AddLink(SqlServerResourceName, 
                new EnvironmentVariableMapping("SQL_SERVER", "#NAME#"),
                new EnvironmentVariableMapping("MSSQL_SA_PASSWORD", "#MSSQL_SA_PASSWORD#"));
    }
}

