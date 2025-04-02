using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Squadron;

public class ComposeResourceManager
{
    private readonly ComposeResourceSettings _settings;
    private readonly List<string> _environmentVariables = new List<string>();

    public ComposeResourceManager(ComposeResourceSettings settings, string name)
    {
        _settings = settings;
        ResourceSettings = settings.Resources.First(x => x.Name == name);
        _environmentVariables.AddRange(settings.GlobalEnvionmentVariables);
    }

    public ComposableResourceSettings ResourceSettings { get; internal set; }
    public ContainerResourceSettings ContainerSettings { get; private set; }
    public Dictionary<string, string> Exports { get; private set; }
    public IComposableResource Resource { get; set; }
    public IReadOnlyList<string> EnvironmentVariables => _environmentVariables;
        
    public void AddEnvironmentVariable(params string[] variable)
    {
        _environmentVariables.AddRange(variable);
    }

    internal async Task StartAsync()
    {
        if (ResourceSettings.Type == ComposableResourceType.Container)
        {
            var builder = ContainerResourceBuilder.New();
            ResourceSettings.ContainerOptions.Configure(builder);
            BuildResourceInstance();
            Resource.SetEnvironmentVariables(EnvironmentVariables.ToList());

            // Give over the networks if resource is not generic
            var networks = new List<string>();
            if(!IsResourceGenericType())
            {
                networks.AddRange(builder.Build().Networks);
            }
                
            networks.Add(_settings.Identifier);
            Resource.SetNetworks(networks);

            await Resource.InitializeAsync();
            Exports = Resource.GetComposeExports();
        }
    }

    private void BuildResourceInstance()
    {
        var composableOptions = (IComposableResourceOption)ResourceSettings.ContainerOptions;
        Type activateType = composableOptions.ResourceType;

        if (IsResourceGenericType())
        {
            activateType = composableOptions.ResourceType
                .MakeGenericType(ResourceSettings.ContainerOptions.GetType());
        }
        Resource = (IComposableResource)Activator.CreateInstance(activateType);
    }

    private bool IsResourceGenericType() =>
        ((IComposableResourceOption)ResourceSettings.ContainerOptions)
        .ResourceType.IsGenericTypeDefinition;

    internal async Task StopAsync()
    {
        await Resource.DisposeAsync();
    }
}