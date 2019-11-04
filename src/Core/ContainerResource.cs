using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        /// The resource options
        /// </summary>
        protected TOptions ResourceOptions = null;

        private List<string> _composeVariables;

        /// <summary>
        /// Initializes the resources
        /// </summary>
        public async virtual Task InitializeAsync()
        {
            ResourceOptions = new TOptions();
            var builder = ContainerResourceBuilder.New();
            ResourceOptions.Configure(builder);
            Settings = builder.Build();

            SetComposeVariables();
            OnSettingsBuilded(Settings);
            ValidateSettings(Settings);

            DockerConfiguration dockerConfig = Settings.DockerConfigResolver();

            Manager = new DockerContainerManager(Settings, dockerConfig);
            Initializer = new ContainerInitializer(Manager, Settings);
            await Manager.CreateAndStartContainerAsync();
        }

        private void SetComposeVariables()
        {
            if (_composeVariables != null)
            {
                foreach (var envVar in _composeVariables)
                {
                    Settings.EnvironmentVariables.Add(envVar);
                }
            }
        }

        public void SetEnvironmentVariables(IEnumerable<string> variables)
        {
            _composeVariables = variables.ToList();
        }


        private void ValidateSettings(ContainerResourceSettings settings)
        {
            if (string.IsNullOrEmpty(settings.Name))
                throw new ArgumentException("Can not be null or empty.", nameof(settings.Name));

            if (string.IsNullOrEmpty(settings.Image))
                throw new ArgumentException("Can not be null or empty.", nameof(settings.Image));

            if (settings.InternalPort == 0)
                throw new ArgumentException("Can not be 0", nameof(settings.InternalPort));
        }

        /// <summary>
        /// Called when after settings are build
        /// </summary>
        /// <param name="settings">The settings.</param>
        protected virtual void OnSettingsBuilded(ContainerResourceSettings settings)
        { }


        public async Task PauseContainer(TimeSpan pauseTime)
        {



        }

        public virtual Dictionary<string, string> GetComposeExports()
        {
            return new Dictionary<string, string>
            {
                { "ADDRESS", Manager.Instance.Address },
                { "PORT", Manager.Instance.HostPort.ToString() },
            };
        }


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
