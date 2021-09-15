using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;
using Xunit;

namespace Squadron
{
    public class FtpServerResource : FtpServerResource<FtpServerDefaultOptions>
    {

    }

    /// <summary>
    /// Represents a FtpServer resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class FtpServerResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        public FtpServerConfiguration FtpServerConfiguration { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync().ConfigureAwait(false);
            FtpServerConfiguration = BuildConfiguration(Settings.Username, Settings.Password);

            await Initializer.WaitAsync(
                new FtpServerStatus(FtpServerConfiguration));
        }

        private FtpServerConfiguration BuildConfiguration(string username, string password)
        {
            return new FtpServerConfiguration(
                Manager.Instance.Address,
                Manager.Instance.HostPort,
                username,
                password);
        }

        public async Task UploadAsync(
            Stream stream,
            string fileName,
            string directory,
            CancellationToken cancellationToken)
        {
            using IFtpClient ftpClient = new FtpClient(
                FtpServerConfiguration.Host,
                FtpServerConfiguration.Port,
                FtpServerConfiguration.Username,
                FtpServerConfiguration.Password);

            try
            {
                await ftpClient.ConnectAsync(token: cancellationToken);

                await ftpClient.UploadAsync(
                    stream,
                    Path.Combine(directory, fileName),
                    token: cancellationToken);
            }
            finally
            {
                await ftpClient.DisconnectAsync(token: cancellationToken);
            }
        }

        public async Task<byte[]> DownloadAsync(
            string fileName,
            string directory,
            CancellationToken cancellationToken)
        {
            using IFtpClient ftpClient = new FtpClient(
                FtpServerConfiguration.Host,
                FtpServerConfiguration.Port,
                FtpServerConfiguration.Username,
                FtpServerConfiguration.Password);

            try
            {
                await ftpClient.ConnectAsync(token: cancellationToken);

                return await ftpClient.DownloadAsync(
                    Path.Combine(directory, fileName),
                    token: cancellationToken);
            }
            finally
            {
                await ftpClient.DisconnectAsync(token: cancellationToken);
            }
        }
    }
}
