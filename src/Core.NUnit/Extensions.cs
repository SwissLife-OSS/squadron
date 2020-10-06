using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squadron
{
    internal static class Extensions
    {
        public static ISquadronAsyncLifetime TryGetByType(
            this IList<ISquadronAsyncLifetime> list,
            Type type)
        {
            return list.FirstOrDefault(p => p.GetType() == type);
        }
    }
}
