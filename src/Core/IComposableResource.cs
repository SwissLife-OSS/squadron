using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Squadron
{
    public interface IComposableResource
    {
        Dictionary<string, string> GetComposeExports();

        Task InitializeAsync();

        Task DisposeAsync();

        void SetEnvironmentVariables(IEnumerable<string> variables);
    }

    public interface IComposableResourceOption
    {
        Type ResourceType { get; }
    }


}
