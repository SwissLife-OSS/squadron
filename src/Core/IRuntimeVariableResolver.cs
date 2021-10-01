using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    internal interface IVariableResolver
    {
        T Resolve<T>(string dynamicVariableName);
        bool CanHandle(VariableType type);
    }
}
