using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron.AzureKeyVault.Tests
{
    public class AzureKeyVaultCreateNewResourceTests
        : IClassFixture<AzureKeyVaultResource<AzureKeyVaultNewOptions>>
    {
        private readonly AzureKeyVaultResource<AzureKeyVaultNewOptions> _resource;

        public AzureKeyVaultCreateNewResourceTests(
            AzureKeyVaultResource<AzureKeyVaultNewOptions> resource)
        {
            _resource = resource;
        }

        [Fact(Skip = "Can not run without Azure credentials")]
        public async Task PrepareVault_CreateNew_NoError()
        {
            // Arrange
            // In Fixture

            // Act
            Uri vaultUri = _resource.VaultUri;

            // Assert
            vaultUri.Should().NotBeNull();
        }
    }

    public class AzureKeyVaultNewOptions : AzureKeyVaultOptions
    {
        public override void Configure(KeyVaultOptionsBuilder builder)
        {
            builder.SetConfigResolver(TestAzureConfigResolver.Resolver);
        }
    }
}
