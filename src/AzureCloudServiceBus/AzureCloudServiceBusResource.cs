using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
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
                         IAzureResourceConfigurationProvider,
                         new()
    {
        private ServiceBusManager _serviceBusManager;

        private ServiceBusModel _serviceBusOptions;
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
        /// Get a TopicClient
        /// </summary>
        /// <param name="name">Topic name</param>
        /// <param name="retryPolicy">Retry policy</param>
        /// <returns></returns>
        public ITopicClient GetTopicClient(string name,
                                           RetryPolicy retryPolicy = null)
        {
            var topicName = GetTopic(name);
            return new TopicClient(ConnectionString, topicName, retryPolicy);
        }


        /// <summary>
        /// Creates a new topic
        /// </summary>
        /// <param name="configure">The builder.</param>
        /// <returns>Client to access the created topic</returns>
        public async Task<ITopicClient> CreateTopicAsync(Action<ServiceBusTopicBuilder> configure)
        {
            var builder = ServiceBusTopicBuilder.New();
            configure(builder);
            ServiceBusTopicModel topic = builder.Build();

            await CreateTopicAsync(topic);
            _serviceBusOptions.Topics.Add(topic);
            return GetTopicClient(topic.Name);
        }

        /// <summary>
        /// Get a SubscriptionClient
        /// </summary>
        /// <param name="topic">Topic name</param>
        /// <param name="name">Subscription name</param>
        /// <param name="receiveMode">Receive Mode</param>
        /// <param name="retryPolicy">Retry Policy</param>
        /// <returns></returns>
        public ISubscriptionClient GetSubscriptionClient(
            string topic,
            string name,
            ReceiveMode receiveMode = ReceiveMode.PeekLock,
            RetryPolicy retryPolicy = null)
        {
            var topicName = GetTopic(topic);
            return new SubscriptionClient(
                ConnectionString,
                topicName,
                name,
                receiveMode,
                retryPolicy);
        }

        /// <summary>
        /// Get a QueueClient
        /// </summary>
        /// <param name="name">Queue name</param>
        /// <param name="receiveMode">Receive Mode</param>
        /// <param name="retryPolicy">Retry Policy</param>
        /// <returns></returns>
        public IQueueClient GetQueueClient(string name,
                                   ReceiveMode receiveMode = ReceiveMode.PeekLock,
                                   RetryPolicy retryPolicy = null)
        {
            var queueName = GetQueue(name);

            return new QueueClient(ConnectionString, queueName, receiveMode, retryPolicy);
        }

        private string GetTopic(string name)
        {
            ServiceBusTopicModel topic = _serviceBusOptions.Topics
                              .FirstOrDefault(x => x.Name.Equals(name,
                                              StringComparison.InvariantCultureIgnoreCase));

            if (topic == null)
                throw new InvalidOperationException($"No topic found with name: {name}");

            return topic.CreatedName;
        }

        private string GetQueue(string name)
        {
            ServiceBusQueueModel queue = _serviceBusOptions.Queues
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
        }

        private void BuildOptions()
        {
            var builder = ServiceBusOptionsBuilder.New();
            var options = new TOptions();
            options.Configure(builder);
            _serviceBusOptions = builder.Build();
        }

        private void InitializeServiceBusManager()
        {
            _serviceBusManager = new ServiceBusManager(
                    AzureConfig.Credentials,
                    new AzureResourceIdentifier
                    {
                        SubscriptionId = AzureConfig.SubscriptionId,
                        ResourceGroupName = AzureConfig.ResourceGroup,
                        Name = _serviceBusOptions.Namespace
                    });
        }

        private async Task PrepareNamespaceAsync()
        {
            if (_serviceBusOptions.Namespace == null)
            {
                _serviceBusOptions.ProvisioningMode = ServiceBusProvisioningMode.CreateAndDelete;
                _serviceBusOptions.Namespace = await
                    _serviceBusManager.CreateNamespaceAsync(AzureConfig.DefaultLocation);
            }
        }

        private async Task PrepareQueuesAsync()
        {
            foreach (ServiceBusQueueModel queue in _serviceBusOptions.Queues)
            {
                if (_serviceBusOptions.ProvisioningMode == ServiceBusProvisioningMode.UseExisting)
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
            foreach (ServiceBusTopicModel topic in _serviceBusOptions.Topics)
            {
                _messageSink.OnMessage(
                    new DiagnosticMessage($"Creating topic {topic.CreatedName}"));
                await CreateTopicAsync(topic);
            }
        }

        private async Task<string> CreateTopicAsync(ServiceBusTopicModel topic)
        {
            if (_serviceBusOptions.ProvisioningMode == ServiceBusProvisioningMode.UseExisting)
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
                if (_serviceBusOptions.ProvisioningMode == ServiceBusProvisioningMode.CreateAndDelete)
                {
                    await _serviceBusManager.DeleteNamespaceAsync();
                }
                else
                {
                    foreach (ServiceBusTopicModel topic in _serviceBusOptions.Topics)
                    {
                        await _serviceBusManager.DeleteTopic(topic.CreatedName);
                    }
                    foreach (ServiceBusQueueModel queue in _serviceBusOptions.Queues)
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
