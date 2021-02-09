using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron
{
    /// <summary>
    /// Class which is responsible to initialize containers
    /// </summary>
    public class ContainerInitializer
    {
        private readonly IDockerContainerManager _manager;
        private readonly ContainerResourceSettings _settings;
        private readonly TimeSpan _consumeLogsInterval = TimeSpan.FromSeconds(3);
        private readonly TimeSpan _readyTimeout = TimeSpan.FromSeconds(5);

        /// <summary>Initializes a new instance of the <see cref="ContainerInitializer"/> class.</summary>
        /// <param name="manager">The manager.</param>
        /// <param name="settings">The settings.</param>
        public ContainerInitializer(
            IDockerContainerManager manager,
            ContainerResourceSettings settings)
        {
            _manager = manager;
            _settings = settings;
        }

        /// <summary>Waits for a container to be ready</summary>
        /// <param name="statusProvider">The status provider.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Initialize sequence timed-out.</exception>
        public async Task<Status> WaitAsync(IResourceStatusProvider statusProvider)
        {
            using (var cancellation = new CancellationTokenSource())
            {
                cancellation.CancelAfter(_settings.WaitTimeout);
                Status status = Status.NotReady;

                while (!cancellation.IsCancellationRequested && !status.IsReady)
                {
                    try
                    {
                        using (var singleRunCancellation = new CancellationTokenSource())
                        {
                            singleRunCancellation.CancelAfter(_readyTimeout);
                            status = await statusProvider.IsReadyAsync(singleRunCancellation.Token);
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.TraceWarning($"Container status error. {_settings.Name} -> {ex.Message}");
                    }

                    if (!status.IsReady)
                    {
                        await _manager.ConsumeLogsAsync(_consumeLogsInterval);
                        Trace.TraceWarning($"Container is not yet {_settings.Name} not ready. --> {status.Message}");
                    }
                }

                if (!status.IsReady)
                {
                    string logs = string.Join("\n",_manager.Instance.Logs.Distinct());
                    throw new InvalidOperationException(
                         $"Initialize sequence timed-out. {status.Message}\r\n{logs}");
                }

                return status;
            }
        }
    }
}
