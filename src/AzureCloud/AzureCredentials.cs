namespace Squadron.AzureCloud
{
    /// <summary>
    /// Azure Active Directory Credentials
    /// https://docs.microsoft.com/en-us/azure/active-directory/develop/app-objects-and-service-principals
    /// </summary>
    public class AzureCredentials
    {
        /// <summary>
        /// Azure Active Directory TenantId
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// Azure AD Service Principal ClientId
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Azure AD Service Principal Secret
        /// </summary>
        public string Secret { get; set; }
    }
}
