using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Squadron
{
    public class SFtpServerStatus : IResourceStatusProvider
    {
        private readonly SFtpServerConfiguration _ftpServerConfiguration;

        public SFtpServerStatus(SFtpServerConfiguration ftpServerConfiguration)
        {
            _ftpServerConfiguration = ftpServerConfiguration;
        }

        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            var connectionInfo = new ConnectionInfo(
                _ftpServerConfiguration.Host,
                _ftpServerConfiguration.Port,
                _ftpServerConfiguration.Username,
                new PasswordAuthenticationMethod(
                    _ftpServerConfiguration.Username,
                    _ftpServerConfiguration.Password));

            using (var client = new SftpClient(connectionInfo))
            {
                client.Connect();
                client.ListDirectory(_ftpServerConfiguration.Directory);
                ConnectionInfo info = client.ConnectionInfo;

                return new Status
                {
                    IsReady = true,
                    Message = $"Ready. Client version: '{info.ClientVersion}'."
                };
            }
        }
    }
}
