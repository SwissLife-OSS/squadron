using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Squadron;

public class ComposeResource<TOptions> : IAsyncLifetime
    where TOptions : ComposeResourceOptions, new()
{
    public ComposeResourceSettings Settings { get; set; }

    protected Dictionary<string, ComposeResourceManager> Managers { get; set; }
        = new Dictionary<string, ComposeResourceManager>();

    public virtual async Task InitializeAsync()
    {
        var options = new TOptions();
        var builder = ComposeResourceBuilder.New();
        options.Configure(builder);
        Settings = builder.Build();

        foreach (var name in BuildStartOrder())
        {
            var mgr = new ComposeResourceManager(Settings, name);
            Managers.Add(name, mgr);

            foreach (ComposeResourceLink link in mgr.ResourceSettings.Links)
            {
                foreach (EnvironmentVariableMapping map in link.EnvironmentVariables)
                {
                    Dictionary<string, string> exports = Managers[link.Name].Exports;
                    mgr.AddEnvironmentVariable($"{map.Name}={GetVariableValue(map.Value, exports)}");
                }
            }
                
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