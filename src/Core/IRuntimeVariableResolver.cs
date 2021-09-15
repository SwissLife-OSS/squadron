using System;
using System.Collections.Generic;
using System.Text;

namespace Squadron
{
    internal interface IRuntimeVariableResolver
    {
        T Resolve<T>(string dynamicVariableName);
        bool CanHandle(RuntimeVariableType type);
    }
}
