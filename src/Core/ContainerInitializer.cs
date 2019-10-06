using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Squadron
{
    public class ContainerInitializer
    {
        private readonly IDockerContainerManager _manager;
        private readonly ContainerResourceSettings _settings;
        private readonly int intervallInSeconds = 3;

        public ContainerInitializer(
            IDockerContainerManager manager,
            ContainerResourceSettings settings)
        {
            _manager = manager;
            _settings = settings;
        }

        public async Task<Status> WaitAsync(
                IResourceStatusProvider statusProvider)
        {
            var timer = Stopwatch.StartNew();
            Status status = Status.NotReady;

            while (timer.Elapsed < _settings.WaitTimeout && !status.IsReady)
            {
                try
                {
                    status = await statusProvider.IsReadyAsync();
                }
                catch ( Exception ex)
                {
                    Trace.TraceWarning($"Container {_settings.Name} not ready: {ex.Message}");
                }
                if (!status.IsReady)
                {
                    await _manager.ConsumeLogsAsync(TimeSpan.FromSeconds(intervallInSeconds));
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
