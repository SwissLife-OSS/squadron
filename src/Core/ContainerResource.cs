using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Squadron
{
    public class ContainerResource<TOptions>
         where TOptions : ContainerResourceOptions, new()
    {
        protected ContainerResourceSettings Settings { get; set; }

        protected IDockerContainerManager Manager = null;

        protected ContainerInitializer Initializer = null;

        public async virtual Task InitializeAsync()
        {
            var options = new TOptions();
            var builder = ContainerResourceBuilder.New();
            options.Configure(builder);

            Settings = builder.Build();
            Manager = new DockerContainerManager(Settings);
            Initializer = new ContainerInitializer(Manager, Settings);
            await Manager.CreateAndStartContainerAsync();
        }

        public async Task DisposeAsync()
        {
            try
            {
                await Manager.StopContainerAsync();
                await Manager.RemoveContainerAsync();
            }
            catch ( Exception ex)
            {
                Trace.TraceWarning($"Could not cleanup container. {ex.Message}");
            }
        }
    }
}
