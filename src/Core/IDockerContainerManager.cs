using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Containers;

namespace Squadron;

/// <summary>
/// DockerContainerManager interface
/// </summary>
public interface IDockerContainerManager : IDisposable
{
    /// <summary>
    /// Gets the container instance.
    /// </summary>
    /// <value>
    /// The instance.
    /// </value>
    ContainerInstance Instance { get; }

    /// <summary>
    /// Gets the underlying Testcontainers container.
    /// </summary>
    IContainer Container { get; }

    /// <summary>
    /// Consumes container logs
    /// </summary>
    /// <param name="timeout">timeout to ready logs</param>
    /// <returns></returns>
    Task<string> ConsumeLogsAsync(TimeSpan timeout);

    /// <summary>
    /// Copies files to the container
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="overrideTargetName">Whether to override the target name.</param>
    Task CopyToContainerAsync(CopyContext context, bool overrideTargetName = false);

    /// <summary>
    /// Creates the and starts the container.
    /// </summary>
    /// <exception cref="ContainerException">Container exited with following logs...</exception>
    Task CreateAndStartContainerAsync();

    /// <summary>
    /// Invokes a command on the container with optional retry support
    /// </summary>
    /// <param name="command">The command to execute.</param>
    /// <param name="retryCount">Number of retry attempts (default: 0).</param>
    /// <param name="retryDelayMs">Delay between retries in milliseconds (default: 1000).</param>
    /// <exception cref="ContainerException"></exception>
    Task<string?> InvokeCommandAsync(string[] command, int retryCount = 0, int retryDelayMs = 1000);

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