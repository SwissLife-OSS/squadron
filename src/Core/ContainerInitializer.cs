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
        private readonly TimeSpan _consumeLogsInterval = TimeSpan.FromSeconds(5);
        private readonly TimeSpan _delayNotReady = TimeSpan.FromSeconds(3);
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
                _settings.Logger.Verbose($"Wait to start for {_settings.WaitTimeout}");
                var retries = 1;
                cancellation.CancelAfter(_settings.WaitTimeout);
                Status status = Status.NotReady;

                while (!cancellation.IsCancellationRequested && !status.IsReady)
                {
                    try
                    {
                        using (var singleRunCancellation = new CancellationTokenSource())
                        {
                            _settings.Logger.Verbose($"Try read container status [run {retries++}]");
                            singleRunCancellation.CancelAfter(_readyTimeout);
                            status = await statusProvider.IsReadyAsync(singleRunCancellation.Token);
                            _settings.Logger.ContainerStatus(status);
                        }
                    }
                    catch (Exception ex)
                    {
                        _settings.Logger.Verbose("Container status error", ex);
                    }

                    if (!status.IsReady)
                    {
                        await Task.Delay(_delayNotReady, cancellation.Token);
                        _settings.Logger.Verbose("Container is not ready");
                    }
                }

                if (!status.IsReady)
                {
                    await _manager.ConsumeLogsAsync(_consumeLogsInterval);
                    throw new InvalidOperationException(
                        $"Initialize sequence timed-out (see test output). {status.Message}");
                }

                return status;
            }
        }
    }
}
