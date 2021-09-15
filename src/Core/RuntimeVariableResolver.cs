using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squadron
{
    internal class RuntimeVariableResolver
    {
        private readonly IList<IRuntimeVariableResolver> _resolvers;
        private readonly IList<RuntimeVariable> _runtimeVariables;

        public RuntimeVariableResolver(IList<RuntimeVariable> runtimeVariables)
        {
            _resolvers = new List<IRuntimeVariableResolver>
            {
                new DynamicPortRuntimeVariableResolver()
            };
            _runtimeVariables = runtimeVariables;
        }

        internal T Resolve<T>(string variableName)
        {
            RuntimeVariable variable = GetRuntimeVariable(variableName);

            foreach (IRuntimeVariableResolver resolver in _resolvers)
            {
                if (resolver.CanHandle(variable.Type))
                {
                    return resolver.Resolve<T>(variableName);
                }
            }

            throw new NotSupportedException(
                $"Runtime variable type '{variable.Type}' is not supported.");
        }

        private RuntimeVariable GetRuntimeVariable(string name)
        {
            RuntimeVariable runtimeVariable =
                _runtimeVariables
                .FirstOrDefault(
                    p => p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (runtimeVariable == null)
            {
                throw new ContainerException($"Variable name '{name}' not set.");
            }

            return runtimeVariable;
        }
    }
}
