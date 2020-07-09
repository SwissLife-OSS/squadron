using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Squadron
{
    public class ComposeResource<TOptions> : IAsyncLifetime
        where TOptions : ComposeResourceOptions, new()
    {
        public ComposeResourceSettings Settings { get; set; }

        protected Dictionary<string, ComposeResourceManager> Managers { get; set; }
            = new Dictionary<string, ComposeResourceManager>();

        private IDictionary<string, IEnumerable<string>> _networks =
            new Dictionary<string, IEnumerable<string>>();

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
                foreach (ComposeNetwork network in mgr.ResourceSettings.Networks)
                {
                    _networks.Add(network.Name, network.ContainerNames);
                }

                // Get all networks that this container should be part of
                foreach (KeyValuePair<string, IEnumerable<string>> network in
                    _networks.Where(kv => kv.Value.Contains(mgr.ContainerSettings.Name)))
                {
                    mgr.ContainerSettings.Networks.Add(network.Key);
                }

                mgr.EnvironmentVariables = Settings.GlobalEnvionmentVariables.Concat(variables);
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
}
