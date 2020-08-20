using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Squadron
{
    public class ComposeResourceManager
    {
        public ComposableResourceSettings ResourceSettings { get; internal set; }
        public ContainerResourceSettings ContainerSettings { get; private set; }
        public Dictionary<string, string> Exports { get; private set; }

        public IComposableResource Resource { get; set; }
        public IEnumerable<string> EnvironmentVariables { get; internal set; }

        internal async Task StartAsync()
        {
            if (ResourceSettings.Type == ComposableResourceType.Container)
            {
                var builder = ContainerResourceBuilder.New();
                ResourceSettings.ContainerOptions.Configure(builder);
                BuildResourceInstance();
                Resource.SetEnvironmentVariables(EnvironmentVariables.ToList());

                // Give over the networks if resource is not generic
                if(!IsResourceGenericType())
                {
                    var networks = builder.Build().Networks;
                    Resource.SetNetworks(networks);
                }

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
                .ResourceType.IsGenericType;

        internal async Task StopAsync()
        {
            await Resource.DisposeAsync();
        }
    }
}
