using System;
using System.Diagnostics;
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
        private readonly int intervallInSeconds = 3;


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
        public async Task<Status> WaitAsync(
                IResourceStatusProvider statusProvider)
        {
            var timer = Stopwatch.StartNew();
            Status status = Status.NotReady;

            while (timer.Elapsed < _settings.WaitTimeout && !status.IsReady)
            {
                try
                {
                    var cts = new CancellationTokenSource();
                    cts.CancelAfter(TimeSpan.FromSeconds(5));
                    status = await statusProvider.IsReadyAsync(cts.Token);
                }
                catch ( Exception ex)
                {
                    Trace.TraceWarning($"Container status error. {_settings.Name} -> {ex.Message}");
                }
                if (!status.IsReady)
                {
                    await _manager.ConsumeLogsAsync(TimeSpan.FromSeconds(intervallInSeconds));
                    Trace.TraceWarning($"Container is not yet {_settings.Name} not ready. --> {status.Message}");
                }
            }

            if (!status.IsReady)
            {
                throw new InvalidOperationException(
                    $"Initialize sequence timed-out. {status.Message}\r\n{_manager.Instance.Logs}");
            }

            return status;
        }
    }
}
