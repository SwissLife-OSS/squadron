using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Squadron
{
    /// <summary>
    /// Default Generic container resource options
    /// </summary>
    public class GenericContainerOptions : ContainerResourceOptions, IComposableResourceOption
    {
        /// <summary>
        /// Configure resource options
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(ContainerResourceBuilder builder)
        {
            builder.Name("generic");
            SetTcpStatusChecker();
        }

        public void Configure(ContainerResourceBuilder builder, string httpStatusCheckPath)
        {
            Configure(builder);
            ConfigureHttpStatusChecker(httpStatusCheckPath);
        }

        public void Configure(ContainerResourceBuilder builder,
            Func<ContainerAddress, CancellationToken, Task<Status>> statusChecker)
        {
            Configure(builder);
            StatusChecker = statusChecker;
        }


        /// <summary>
        /// The status checker
        /// </summary>
        internal Func<ContainerAddress, CancellationToken, Task<Status>> StatusChecker;

        public Type ResourceType => typeof(GenericContainerResource<>);

        /// <summary>
        /// Configures the status checker.
        /// When no status check is configured a default TCP check will be used
        /// </summary>
        /// <param name="checker">The checker.</param>
        public void ConfigureStatusChecker(
            Func<ContainerAddress, CancellationToken, Task<Status>> checker)
        {
            StatusChecker = checker;
        }

        private void SetTcpStatusChecker()
        {
            StatusChecker = CreateTcpStatusChecker();
        }

        public void ConfigureHttpStatusChecker(string path="/")
        {
            StatusChecker = CreateHttpStatusChecker(path);
        }

        private Func<ContainerAddress, CancellationToken, Task<Status>> CreateTcpStatusChecker()
        {
            return async (address, CancellationToken) =>
            {
                using (var tcpClient = new TcpClient())
                {
                    await tcpClient.ConnectAsync(address.Address, address.Port);
                    return new Status
                    {
                        IsReady = tcpClient.Connected
                    };
                }
            };
        }

        private Func<ContainerAddress, CancellationToken, Task<Status>> CreateHttpStatusChecker(
            string path)
        {
            return async (address, CancellationToken) =>
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri($"http://{address.Address}:{address.Port}");

                HttpResponseMessage response = await client.GetAsync(path);
                return new Status
                {
                    IsReady = response.IsSuccessStatusCode
                };
            };
        }
    }
}
