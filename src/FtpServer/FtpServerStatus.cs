using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;

namespace Squadron
{
    public class FtpServerStatus : IResourceStatusProvider
    {
        private readonly FtpServerConfiguration _ftpServerConfiguration;

        public FtpServerStatus(FtpServerConfiguration ftpServerConfiguration)
        {
            _ftpServerConfiguration = ftpServerConfiguration;
        }

        public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
        {
            using IFtpClient ftpClient = new FtpClient(
                _ftpServerConfiguration.Host,
                _ftpServerConfiguration.Port,
                _ftpServerConfiguration.Username,
                _ftpServerConfiguration.Password);
            
            try
            {
                await ftpClient.ConnectAsync(cancellationToken);
                await ftpClient.GetListingAsync("/", cancellationToken);

                return new Status { IsReady = true, Message = "Ready" };
            }
            catch (Exception ex)
            {
                return new Status { IsReady = false, Message = ex.Message };
            }
            finally
            {
                await ftpClient.DisconnectAsync(cancellationToken);
            }
        }
    }
}
