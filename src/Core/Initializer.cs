using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Squadron
{
    /// <summary>
    /// Runs operations against a specific resource
    /// </summary>
    public static class Initializer
    {
        /// <summary>
        /// Waits a specific resource to initialize by checking his status.
        /// </summary>
        public static async Task<Status> WaitAsync(
            IResourceStatusProvider statusProvider,
            IImageSettings settings)
        {
            return await WaitAsync(
                statusProvider, TimeSpan.FromSeconds(30), settings);
        }

        internal static async Task<Status> WaitAsync(
            IResourceStatusProvider statusProvider,
            TimeSpan timeout,
            IImageSettings settings)
        {
            Stopwatch timer = Stopwatch.StartNew();
            Status status = Status.NotReady;

            while (timer.Elapsed < timeout && !status.IsReady)
            {
                try
                {
                    status = await statusProvider.IsReadyAsync();
                }
                catch
                {
                    // We are not interested in any client errors
                }
                if (!status.IsReady)
                    await Task.Delay(TimeSpan.FromSeconds(2));
            }

            if (!status.IsReady)
            {
                throw new InvalidOperationException(
                    $"Initialize sequence timed-out. {status.Message}\r\n{settings.Logs}");
            }

            return status;
        }
    }
}
