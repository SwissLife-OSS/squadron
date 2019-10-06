#if NET46
using Microsoft.SqlServer.Dac;

namespace Squadron
{
    internal static class DacpacOptions
    {
        internal static DacExtractOptions Extract { get; } = new DacExtractOptions
        {
            IgnorePermissions = true,
            IgnoreUserLoginMappings = true
        };

        internal static DacDeployOptions Deploy { get; } = new DacDeployOptions
        {
            IgnorePermissions = true,
            IgnoreUserSettingsObjects = true,
            IgnoreLoginSids = true,
            IgnoreRoleMembership = true,
            ExcludeObjectTypes = new[]
            {
                ObjectType.Users,
                ObjectType.Logins,
                ObjectType.RoleMembership,
                ObjectType.ServerRoles,
                ObjectType.ApplicationRoles,
                ObjectType.DatabaseRoles,
                ObjectType.ServerRoleMembership,
            }
        };
    }
}
#endif
