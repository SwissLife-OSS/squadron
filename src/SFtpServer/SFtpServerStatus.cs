using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;

namespace Squadron;

public class SFtpServerStatus(SFtpServerConfiguration ftpServerConfiguration) : IResourceStatusProvider
{
    public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
    {
        var connectionInfo = new ConnectionInfo(
            ftpServerConfiguration.Host,
            ftpServerConfiguration.Port,
            ftpServerConfiguration.Username,
            new PasswordAuthenticationMethod(
                ftpServerConfiguration.Username,
                ftpServerConfiguration.Password));

        using (var client = new SftpClient(connectionInfo))
        {
            client.Connect();
            client.ListDirectory(ftpServerConfiguration.Directory);
            ConnectionInfo info = client.ConnectionInfo;

            return new Status
            {
                IsReady = true,
                Message = $"Ready. Client version: '{info.ClientVersion}'."
            };
        }
    }
}