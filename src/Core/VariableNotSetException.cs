using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Squadron
{
    public class VariableNotSetException : Exception
    {
        public VariableNotSetException(VariableType variableType)
            : base($"Variable of type {variableType} was not set in container options.")
        {
        }
    }
}
