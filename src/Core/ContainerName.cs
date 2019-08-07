using System;

namespace Squadron
{
    public static class ContainerName
    {
        public static string Create() =>
            ("test_" + Guid.NewGuid().ToString("N")).ToLowerInvariant();
    }
}
