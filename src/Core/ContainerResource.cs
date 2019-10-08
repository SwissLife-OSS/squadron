using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Squadron
{

    /// <summary>
    /// Base resource for container based resources
    /// </summary>
    /// <typeparam name="TOptions">The type of the options.</typeparam>
    public class ContainerResource<TOptions>
         where TOptions : ContainerResourceOptions, new()
    {
        /// <summary>
        /// Gets or sets the continer settings.
        /// </summary>
        /// <value>
        /// The settings.
        /// </value>
        protected ContainerResourceSettings Settings { get; set; }


        /// <summary>
        /// The container manager
        /// </summary>
        protected IDockerContainerManager Manager = null;


        /// <summary>
        /// The container initializer
        /// </summary>
        protected ContainerInitializer Initializer = null;

        /// <summary>
        /// Initializes the resources
        /// </summary>
        public async virtual Task InitializeAsync()
        {
            var options = new TOptions();
            var builder = ContainerResourceBuilder.New();
            options.Configure(builder);
            Settings = builder.Build();
            OnSettingsBuilded(Settings);

            Manager = new DockerContainerManager(Settings);
            Initializer = new ContainerInitializer(Manager, Settings);
            await Manager.CreateAndStartContainerAsync();
        }


        /// <summary>
        /// Called when after settings are build
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected virtual void OnSettingsBuilded(ContainerResourceSettings settings)
        { }


        /// <summary>
        /// Cleans up the resource
        /// </summary>
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
