using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Squadron;

public interface IComposableResource
{
    Dictionary<string, string> GetComposeExports();

    ValueTask InitializeAsync();

    ValueTask DisposeAsync();

    void SetEnvironmentVariables(IEnumerable<string> variables);

    void SetNetworks(IEnumerable<string> networkName);
}

public interface IComposableResourceOption
{
    Type ResourceType { get; }
}