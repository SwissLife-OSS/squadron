using System;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace Squadron
{
    public interface IDockerContainerManager
    {
        ContainerInstance Instance { get; }

        Task<string> ConsumeLogsAsync(TimeSpan timeout);
        Task CopyToContainer(CopyContext context);
        Task CreateAndStartContainerAsync();
        Task InvokeCommandAsync(ContainerExecCreateParameters parameters);
        Task RemoveContainerAsync();
        Task<bool> StopContainerAsync();
    }
}