using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    internal static class UniqueNameGenerator
    {
        internal static string Create(string prefix)
        {
            return $"{prefix}_{Guid.NewGuid():N}";
        }
    }
}
