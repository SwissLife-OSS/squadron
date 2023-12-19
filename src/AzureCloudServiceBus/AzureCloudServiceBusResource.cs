using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core.Pipeline;
using Azure.Messaging.ServiceBus;
using Squadron.AzureCloud;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Squadron
{
    /// <summary>
    /// Defines a Azure Cloud ServiceBus namespace 
    /// </summary>
    /// <typeparam name="TOptions">Option to initialize the resource</typeparam>
    public class AzureCloudServiceBusResource<TOptions>
            : AzureResource<TOptions>, IAsyncLifetime
        where TOptions : AzureCloudServiceBusOptions,
                         new()
    {
        private ServiceBusManager _serviceBusManager;

        private ServiceBusModel _serviceBusModel;
        private readonly IMessageSink _messageSink;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCloudServiceBusResource{TOptions}"/> class.
        /// </summary>
        /// <param name="messageSink">The message sink.</param>
        public AzureCloudServiceBusResource(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        /// <summary>
        /// ConnectionString to access the Azure ServiceBus
        /// </summary>
        public string ConnectionString { get; private set; }
        
        /// <summary>
        /// ServiceBus client
        /// </summary>
        public ServiceBusClient ServiceBusClient { get; private set; }

        /// <summary>
        /// Get a ServiceBusSender fot topic
        /// </summary>
        /// <param name="name">Topic name</param>
        /// <returns></returns>
        public ServiceBusSender GetTopicSender(string name)
        {
            var topicName = GetTopic(name);

            return ServiceBusClient.CreateSender(topicName);
        }
        
        /// <summary>
        /// Get a ServiceBusSender fot topic subscription
        /// </summary>
        /// <param name="name">Topic name</param>
        /// <param name="subscriptionName">Subscription name</param>
        /// <param name="receiveMode">Receive mode</param>
        /// <returns></returns>
        public ServiceBusReceiver GetTopicSubscriptionReceiver(
            string name,
            string subscriptionName,
            ServiceBusReceiveMode receiveMode = ServiceBusReceiveMode.PeekLock)
        {
            var topicName = GetTopic(name);

            return ServiceBusClient.CreateReceiver(
                topicName, 
                subscriptionName, 
                new ServiceBusReceiverOptions
            {
                ReceiveMode = receiveMode
            });
        }


        /// <summary>
        /// Creates a new topic
        /// </summary>
        /// <param name="configure">The builder.</param>
        /// <returns>Client to access the created topic</returns>
        public async Task<ServiceBusSender> CreateTopicAsync(Action<ServiceBusTopicBuilder> configure)
        {
            var builder = ServiceBusTopicBuilder.New();
            configure(builder);
            ServiceBusTopicModel topic = builder.Build();

            await CreateTopicAsync(topic);
            _serviceBusModel.Topics.Add(topic);
            
            return GetTopicSender(topic.Name);
        }

        /// <summary>
        /// Get a ServiceBusSender for queue
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <returns></returns>
        public ServiceBusSender GetQueueSender(string name)
        {
            var queueName = GetQueue(name);

            return ServiceBusClient.CreateSender(queueName);
        }
        
        /// <summary>
        /// Get a ServiceBusSender for queue
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <param name="receiveMode">Receive mode</param>
        /// <returns></returns>
        public ServiceBusReceiver GetQueueReceiver(
            string name,
            ServiceBusReceiveMode receiveMode = ServiceBusReceiveMode.PeekLock)
        {
            var queueName = GetQueue(name);

            return ServiceBusClient.CreateReceiver(
                queueName, 
                new ServiceBusReceiverOptions
                {
                    ReceiveMode = receiveMode
                });
        }

        private string GetTopic(string name)
        {
            ServiceBusTopicModel topic = _serviceBusModel.Topics
                              .FirstOrDefault(x => x.Name.Equals(name,
                                              StringComparison.InvariantCultureIgnoreCase));

            if (topic == null)
                throw new InvalidOperationException($"No topic found with name: {name}");

            return topic.CreatedName;
        }

        private string GetQueue(string name)
        {
            ServiceBusQueueModel queue = _serviceBusModel.Queues
                              .FirstOrDefault(x => x.Name.Equals(name,
                                              StringComparison.InvariantCultureIgnoreCase));

            if (queue == null)
                throw new InvalidOperationException($"No queue found with name: {name}");

            return queue.CreatedName;
        }

        /// <summary>
        /// Initialize the resource
        /// </summary>
        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            BuildOptions();
            InitializeServiceBusManager();
            await PrepareNamespaceAsync();
            await PrepareTopicsAsync();
            await PrepareQueuesAsync();

            ConnectionString = await _serviceBusManager.GetConnectionString();
            ServiceBusClient = new ServiceBusClient(ConnectionString);
        }

        private void BuildOptions()
        {
            var builder = ServiceBusOptionsBuilder.New();
            var options = new TOptions();
            options.Configure(builder);
            LoadResourceConfiguration(builder);
            _serviceBusModel = builder.Build();
        }

        private void InitializeServiceBusManager()
        {
            _serviceBusManager = new ServiceBusManager(
                    AzureConfig.Credentials,
                    new AzureResourceIdentifier
                    {
                        SubscriptionId = AzureConfig.SubscriptionId,
                        ResourceGroupName = AzureConfig.ResourceGroup,
                        Name = _serviceBusModel.Namespace
                    });
        }

        private async Task PrepareNamespaceAsync()
        {
            if (_serviceBusModel.Namespace == null)
            {
                _serviceBusModel.ProvisioningMode = ServiceBusProvisioningMode.CreateAndDelete;
                _serviceBusModel.Namespace = await
                    _serviceBusManager.CreateNamespaceAsync(AzureConfig.DefaultLocation);
            }
        }

        private async Task PrepareQueuesAsync()
        {
            foreach (ServiceBusQueueModel queue in _serviceBusModel.Queues)
            {
                if (_serviceBusModel.ProvisioningMode == ServiceBusProvisioningMode.UseExisting)
                {
                    queue.CreatedName = $"{queue.Name}_{DateTime.UtcNow.Ticks}";
                }
                else
                {
                    queue.CreatedName = queue.Name;
                }
                await _serviceBusManager.CreateQueueAsync(queue.CreatedName);
            }
        }

        private async Task PrepareTopicsAsync()
        {
            foreach (ServiceBusTopicModel topic in _serviceBusModel.Topics)
            {
                _messageSink.OnMessage(
                    new DiagnosticMessage($"Creating topic {topic.CreatedName}"));
                await CreateTopicAsync(topic);
            }
        }

        private async Task<string> CreateTopicAsync(ServiceBusTopicModel topic)
        {
            if (_serviceBusModel.ProvisioningMode == ServiceBusProvisioningMode.UseExisting)
            {
                topic.CreatedName = $"{topic.Name}_{DateTime.UtcNow.Ticks}";
            }
            else
            {
                topic.CreatedName = topic.Name;
            }
            await _serviceBusManager.CreateTopic(topic);
            return topic.CreatedName;
        }

        /// <summary>
        /// Cleans up the resource
        /// </summary>
        public async Task DisposeAsync()
        {
            try
            {
                await ServiceBusClient.DisposeAsync();
                
                if (_serviceBusModel.ProvisioningMode == ServiceBusProvisioningMode.CreateAndDelete)
                {
                    await _serviceBusManager.DeleteNamespaceAsync();
                }
                else
                {
                    foreach (ServiceBusTopicModel topic in _serviceBusModel.Topics)
                    {
                        await _serviceBusManager.DeleteTopic(topic.CreatedName);
                    }
                    foreach (ServiceBusQueueModel queue in _serviceBusModel.Queues)
                    {
                        await _serviceBusManager.DeleteQueue(queue.CreatedName);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceWarning($"Error cleaning up azure resources: {ex.Message}");
                //do not fail test
            }
        }
    }
}
