using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Squadron.AzureCloud;

namespace Squadron.AzureCloud
{
    public partial class AzureAdTokenManager
    {
        public async Task<TokenCredentials> RequestTokenAsync(AzureCredentials azureCredentials)
        {
            var context = new AuthenticationContext(
                $"https://login.microsoftonline.com/{azureCredentials.TenantId}");
            var clientCredential = new ClientCredential(
                azureCredentials.ClientId,
                azureCredentials.Secret);

            AuthenticationResult authenticationResult =
                await context.AcquireTokenAsync("https://management.azure.com/", clientCredential)
                    .ConfigureAwait(false);
            var tokenCredentials = new TokenCredentials(authenticationResult.AccessToken);

            return tokenCredentials;
        }
    }
}
