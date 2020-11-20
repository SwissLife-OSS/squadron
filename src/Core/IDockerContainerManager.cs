using System;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace Squadron
{
    /// <summary>
    /// DockerContainerManager interface
    /// </summary>
    public interface IDockerContainerManager
    {
        /// <summary>
        /// Gets the container instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        ContainerInstance Instance { get; }

        /// <summary>
        /// Consumes container logs
        /// </summary>
        /// <param name="timeout">timeout to ready logs</param>
        /// <returns></returns>
        Task<string> ConsumeLogsAsync(TimeSpan timeout);

        /// <summary>
        /// Copies files to the contaier
        /// </summary>
        /// <param name="context">The context.</param>
        Task CopyToContainerAsync(CopyContext context ,bool overrideTargetName = false);

        /// <summary>
        /// Creates the and starts the container.
        /// </summary>
        /// <exception cref="ContainerException">Container exited with following logs...</exception>
        Task CreateAndStartContainerAsync();

        /// <summary>
        /// Invokes a command on the container
        /// </summary>
        /// <param name="parameters">Command parameter</param>
        /// <exception cref="ContainerException"></exception>
        Task InvokeCommandAsync(ContainerExecCreateParameters parameters);

        /// <summary>
        /// Removes the container.
        /// </summary>
        Task RemoveContainerAsync();

        /// <summary>
        /// Stops the container.
        /// </summary>
        /// <returns></returns>
        Task<bool> StopContainerAsync();
    }
}
