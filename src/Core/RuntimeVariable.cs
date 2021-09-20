using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    public class Variable
    {
        public Variable
            (
            string name,
            VariableType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }
        public VariableType Type { get; }
    }
}
