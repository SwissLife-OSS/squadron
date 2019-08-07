using System.Threading.Tasks;

namespace Squadron
{
    public class ResourceBase<T>
        where T : IImageSettings, new()
    {
        private bool _disposed;

        protected ResourceBase()
        {
            Settings = new T();
        }

        public IImageSettings Settings { get; }

        /// <summary>
        /// Initialize container
        /// </summary>
        protected async Task StartContainerAsync()
        {
            await Container.Start(Settings);
        }

        /// <summary>
        /// Stop and remote container
        /// </summary>
        /// <returns></returns>
        protected async Task StopContainerAsync()
        {
            if (!_disposed)
            {
                await Container.Stop(Settings);
                _disposed = true;
            }
        }
    }
}
