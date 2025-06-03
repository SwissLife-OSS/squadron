using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squadron;

internal class VariableResolver(IList<Variable> variables)
{
    private readonly IList<IVariableResolver> _resolvers = new List<IVariableResolver>
    {
        new DynamicPortVariableResolver()
    };

    internal T Resolve<T>(string variableName)
    {
        Variable variable = GetVariable(variableName);

        foreach (IVariableResolver resolver in _resolvers)
        {
            if (resolver.CanHandle(variable.Type))
            {
                return resolver.Resolve<T>(variableName);
            }
        }

        throw new NotSupportedException(
            $"Variable type '{variable.Type}' is not supported.");
    }

    private Variable GetVariable(string name)
    {
        Variable variable =
            variables
                .FirstOrDefault(
                    p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        if (variable == null)
        {
            throw new ContainerException($"Variable name '{name}' not set.");
        }

        return variable;
    }
}