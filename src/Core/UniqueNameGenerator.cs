using System;

namespace Squadron
{
    public static class UniqueNameGenerator
    {
        public static string Create(string prefix)
        {
            return $"{prefix}_{Guid.NewGuid():N}";
        }

        public static string CreateContainerName(string name)
            => $"squa_{name.ToLowerInvariant()}_{DateTime.UtcNow.Ticks}_" +
               $"{Guid.NewGuid().ToString("N").Substring(6)}";

        public static string CreateNetworkName(string container1, string container2)
            => $"squa_network_{container1.ToLowerInvariant()}_{container2.ToLowerInvariant()}_" +
               $"{Guid.NewGuid().ToString("N").Substring(6)}";
    }
}
