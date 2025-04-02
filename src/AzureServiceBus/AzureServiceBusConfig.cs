using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Squadron;

public class AzureServiceBusConfig
{
    private static readonly JsonSerializerOptions _options = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };
    
    protected virtual Queue[] CreateQueues()
    {
        return [];
    }
    
    protected virtual Topic[] CreateTopics()
    {
        return [];
    }

    public string Build()
    {
        var fileName = Path.GetTempFileName();
        
        Queue[] queues = CreateQueues()
            .Concat([new Queue(AzureServiceBusStatus.QueueName)])
            .ToArray();
        
        var config = new
        {
            UserConfig = new UserConfig(
                [new Namespace(queues, CreateTopics())],
                new Logging())
        };

        var serializedConfig = JsonSerializer.Serialize(config, options: _options);
        File.WriteAllText(fileName, serializedConfig);

        return fileName;
    }
}

public record UserConfig(
    Namespace[] Namespaces,
    Logging Logging);
public record Namespace(
    Queue[] Queues,
    Topic[] Topics,
    string Name = "sbemulatorns");
public record Queue(
    string Name,
    QueueProperties? Properties = null);
public record QueueProperties(
    bool DeadLetteringOnMessageExpiration = true,
    string DefaultMessageTimeToLive= "PT1H",
    string DuplicateDetectionHistoryTimeWindow = "PT1M",
    string ForwardDeadLetteredMessagesTo = "",
    string ForwardTo = "",
    string LockDuration= "PT1M",
    int MaxDeliveryCount = 5,
    bool RequiresDuplicateDetection = false,
    bool RequiresSession = false);
public record Topic(
    string Name,
    TopicProperties? Properties = null,
    Subscription[]? Subscriptions = null);
public record TopicProperties(
    string DefaultMessageTimeToLive = "PT1H",
    string DuplicateDetectionHistoryTimeWindow = "PT1M",
    bool RequiresDuplicateDetection = false);
public record Subscription(
    string Name,
    SubscriptionProperties? Properties = null,
    SubscriptionRule[]? Rules = null);
public record SubscriptionProperties(
    bool DeadLetteringOnMessageExpiration = true,
    string DefaultMessageTimeToLive = "PT1H",
    string LockDuration = "PT1M",
    int MaxDeliveryCount = 5,
    string ForwardDeadLetteredMessagesTo = "",
    string ForwardTo = "",
    bool RequiresSession = false);
public record SubscriptionRule(
    string Name,
    TopicRuleProperties? Properties = null);
public record TopicRuleProperties(
    string FilterType,
    CorrelationFilter CorrelationFilter);
public record CorrelationFilter(
    string ContentType,
    string CorrelationId,
    string Label,
    string MessageId,
    string ReplyTo,
    string ReplyToSessionId,
    string SessionId,
    string To);
public record Logging(string Type = "Console");
