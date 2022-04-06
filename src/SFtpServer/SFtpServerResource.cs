using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;
using Xunit;

namespace Squadron
{
    public class SFtpServerResource : SFtpServerResource<SFtpServerDefaultOptions>
    {
    }

    /// <summary>
    /// Represents a FtpServer resource that can be used by unit tests.
    /// </summary>
    /// <seealso cref="IDisposable"/>
    public class SFtpServerResource<TOptions>
        : ContainerResource<TOptions>,
          IAsyncLifetime
        where TOptions : ContainerResourceOptions, new()
    {
        public SFtpServerConfiguration FtpServerConfiguration { get; private set; }

        /// <inheritdoc cref="IAsyncLifetime"/>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync().ConfigureAwait(false);
            FtpServerConfiguration = BuildConfiguration(Settings.Username, Settings.Password);

            await Initializer.WaitAsync(
                new SFtpServerStatus(FtpServerConfiguration));
        }

        private SFtpServerConfiguration BuildConfiguration(string username, string password)
        {
            Settings.KeyValueStore
                .TryGetValue(WellKnown.DirectoryName, out object directoryNameObj);

            if (directoryNameObj is null || directoryNameObj is not string directoryName)
            {
                throw new ContainerException(
                    $"Key {WellKnown.DirectoryName} not set int {nameof(Settings.KeyValueStore)}.");
            }

            return new SFtpServerConfiguration(
                Manager.Instance.Address,
                Manager.Instance.HostPort,
                username,
                password,
                directoryName);
        }

        public void Upload(Stream stream, string fileName, string path)
        {
            using (var client = new SftpClient(ConnectionInfo))
            {
                client.Connect();
                client.UploadFile(stream, $"{path}/{fileName}");
                client.Disconnect();
            }
        }

        public byte[] Download(string fileName, string path)
        {
            using (var client = new SftpClient(ConnectionInfo))
            {
                using var downloadedFileStream = new MemoryStream();
                client.Connect();
                client.DownloadFile($"{path}/{fileName}", downloadedFileStream);
                client.Disconnect();

                return downloadedFileStream.ToArray();
            }
        }

        private ConnectionInfo ConnectionInfo =>
            new ConnectionInfo(
                FtpServerConfiguration.Host,
                FtpServerConfiguration.Port,
                FtpServerConfiguration.Username,
                new PasswordAuthenticationMethod(
                    FtpServerConfiguration.Username,
                    FtpServerConfiguration.Password));

    }
}
