using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Squadron
{
    internal static class SquadronNUnitHelpers
    {
        public static IList<Type> GetFromTypeByInterface(Type type)
        {
            //TODO: filter interfaces
            var interfaces = type.GetInterfaces();
            //TODO: filter generic arguments
            var resourceTypes = interfaces.Select(p => p.GetGenericArguments().First()).ToList();
            return resourceTypes;
        }

        public static IList<FieldInfo> GetFromTypeByAttribute(Type type)
        {
            return type
                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(NUnitSquadronInjectAttribute)))
                .Select(p => p).ToList();
        }
    }
}
