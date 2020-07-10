using System;

namespace Squadron
{
    public static class UniqueNameGenerator
    {
        public static string Create(string prefix)
            => $"{prefix}_{Guid.NewGuid():N}";

        public static string CreateContainerName(string name)
            => $"squa_{name.ToLowerInvariant()}_{DateTime.UtcNow.Ticks}_" +
               $"{Guid.NewGuid().ToString("N").Substring(6)}";

        public static string CreateNetworkName(string name)
            => $"squa_network_{name.ToLowerInvariant()}_" +
               $"{Guid.NewGuid().ToString("N").Substring(6)}";

    }
}
