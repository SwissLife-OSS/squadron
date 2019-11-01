using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

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
                await Resource.InitializeAsync();
                Exports = Resource.GetComposeExports();
            }
        }

        private void BuildResourceInstance()
        {
            var composableOptions = (IComposableResourceOption)ResourceSettings.ContainerOptions;
            Type activateType = composableOptions.ResourceType;

            if (composableOptions.ResourceType.IsGenericType)
            {
                activateType = composableOptions.ResourceType
                    .MakeGenericType(ResourceSettings.ContainerOptions.GetType());
            }
            Resource = (IComposableResource)Activator.CreateInstance(activateType);
        }

        internal async Task StopAsync()
        {
            await Resource.DisposeAsync();
        }
    }


    public class ComposeResource<TOptions> : IAsyncLifetime
        where TOptions : ComposeResourceOptions, new()
    {
        public ComposeResourceSettings Settings { get; set; }

        protected Dictionary<string, ComposeResourceManager> Managers { get; set; }
            = new Dictionary<string, ComposeResourceManager>();

        public async Task InitializeAsync()
        {
            var options = new TOptions();
            var builder = ComposeResourceBuilder.New();
            options.Configure(builder);
            Settings = builder.Build();

            foreach (var name in BuildStartOrder())
            {
                var mgr = new ComposeResourceManager();
                mgr.ResourceSettings = Settings.Resources.First(x => x.Name == name);
                Managers.Add(name, mgr);

                var variables = new List<string>();
                foreach (ComposeResourceLink link in mgr.ResourceSettings.Links)
                {
                    foreach (EnvironmentVariableMapping map in link.EnvironmentVariables)
                    {
                        Dictionary<string, string> exports = Managers[link.Name].Exports;
                        variables.Add($"{map.Name}={GetVariableValue(map.Value, exports)}");
                    }
                }
                mgr.EnvironmentVariables = variables;
                await mgr.StartAsync();
            }
        }


        public TResource GetResource<TResource>(string name)
        {
            ComposeResourceManager manager = Managers[name];
            return (TResource)manager.Resource;
        }

        private string GetVariableValue(string template, Dictionary<string, string> exports)
        {
            var value = template;
            foreach (KeyValuePair<string, string> export in exports)
            {
                value = value.Replace($"#{export.Key}#", export.Value);
            }
            return value;
        }

        public async Task DisposeAsync()
        {
            var stopTasks = new List<Task>();

            foreach (KeyValuePair<string, ComposeResourceManager> mgr in Managers)
            {
                stopTasks.Add(mgr.Value.StopAsync());
            }
            await Task.WhenAll(stopTasks);
        }

        private List<Tuple<int, string>> byLevel = new List<Tuple<int, string>>();

        private IEnumerable<string> BuildStartOrder()
        {
            //TODO: Build dependency graph
            return Settings.Resources.Select(x => x.Name);
        }

        private ComposableResourceSettings GetResourceSetting(string name)
        {
            return Settings.Resources.FirstOrDefault(x => x.Name == name);
        }
    }

    public abstract class ComposeResourceOptions
    {
        public abstract void Configure(ComposeResourceBuilder builder);
    }

    public static class ComposableResourceSettingsExtensions
    {
        public static bool HasLinks(this ComposableResourceSettings settings)
        {
            return settings.Links.Any();
        }

        public static IEnumerable<ComposableResourceSettings> Links(
            this ComposableResourceSettings resource,
            IEnumerable<ComposableResourceSettings> all )
        {
            foreach (ComposeResourceLink link in resource.Links)
            {
                ComposableResourceSettings linkRes = all.FirstOrDefault(x => x.Name == link.Name);
                yield return linkRes;
            }
        }
    }
}
