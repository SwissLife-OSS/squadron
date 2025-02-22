using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Squadron;

public class AzureServiceBusStatus(ServiceBusClient client) :
    IResourceStatusProvider
{
    public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
    {
        try
        {
            var sender = client.CreateSender("queue.1"); // TODO: _squa_status_check_
            await sender.SendMessageAsync(new ServiceBusMessage(), cancellationToken);

            return new Status { IsReady = true };
        }
        catch (Exception ex)
        {
            return new Status { IsReady = false, Message = "Not ready" };
        }
    }
}
