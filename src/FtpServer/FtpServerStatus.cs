using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentFTP;

namespace Squadron;

public class FtpServerStatus(FtpServerConfiguration ftpServerConfiguration) : IResourceStatusProvider
{
    public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
    {
        using IFtpClient ftpClient = new FtpClient(
            ftpServerConfiguration.Host,
            ftpServerConfiguration.Port,
            ftpServerConfiguration.Username,
            ftpServerConfiguration.Password);
            
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