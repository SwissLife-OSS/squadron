using System.Threading.Tasks;

namespace Squadron
{
    /// <summary>
    /// Provides a way to interact with the container.
    /// </summary>
    public static class Container
    {
        /// <summary>
        /// Invoke a given command on the current container in detached.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="settings"></param>
        public static async Task InvokeCommand(
            ICommand command,
            IImageSettings settings)
        {
            await DockerManager.InvokeCommand(
                command.ToContainerExecCreateParameters(),
                settings);
        }

        /// <summary>
        /// Executes a copy command described by the <see cref="CopyContext"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="settings"></param>
        public static async Task CopyTo(
            CopyContext context,
            IImageSettings settings)
        {
            await DockerManager.CopyToContainer(
                context,
                settings);
        }

        internal static async Task Start(
            IImageSettings imageSettings)
        {
            await DockerManager.CreateAndStartContainer(
                imageSettings);
        }

        internal static async Task Stop(
            IImageSettings settings)
        {
            await DockerManager.StopAndRemoveContainer(
                settings.Name);
        }
    }
}
