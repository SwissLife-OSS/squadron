using Squadron.AzureCloud;

namespace Squadron;

public abstract class AzureKeyVaultOptions : AzureResourceOptions
{
    public abstract void Configure(KeyVaultOptionsBuilder builder);
}