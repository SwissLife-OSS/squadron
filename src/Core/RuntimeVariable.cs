using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class RuntimeVariable
    {
        public RuntimeVariable(
            string name,
            RuntimeVariableType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public RuntimeVariableType Type { get; }
    }
}
