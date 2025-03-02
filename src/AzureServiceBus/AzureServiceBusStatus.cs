using System;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace Squadron;

public class AzureServiceBusStatus(ServiceBusClient client) :
    IResourceStatusProvider
{
    public static string QueueName = "squadron.status.check";
    
    public async Task<Status> IsReadyAsync(CancellationToken cancellationToken)
    {
        try
        {
            var sender = client.CreateSender(QueueName);
            var serviceBusMessage = new ServiceBusMessage(BinaryData.FromString("status_check"));
            await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

            return new Status { IsReady = true };
        }
        catch (Exception ex)
        {
            return new Status { IsReady = false, Message = "Not ready" };
        }
    }
}
