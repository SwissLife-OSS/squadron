using System;

namespace Squadron;

/// <summary>
/// ServiceBusTopic builder
/// </summary>
public class ServiceBusTopicBuilder
{
    ServiceBusTopicModel _topic = null;

    private ServiceBusTopicBuilder()
    {
        _topic = new ServiceBusTopicModel();
    }

    private ServiceBusTopicBuilder(string topicName)
    {
        _topic = new ServiceBusTopicModel(topicName);
    }

    /// <summary>
    /// Creates a new empty builder
    /// </summary>
    /// <returns></returns>
    public static ServiceBusTopicBuilder New()
        => new ServiceBusTopicBuilder();


    /// <summary>
    /// Creates a new builder with a topic name
    /// </summary>
    /// <param name="name">The topic name.</param>
    /// <returns></returns>
    public static ServiceBusTopicBuilder New(string name)
        => new ServiceBusTopicBuilder(name);


    /// <summary>
    /// Topic name
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns></returns>
    public ServiceBusTopicBuilder Name(string name)
    {
        _topic.Name = name;
        return this;
    }


    /// <summary>
    /// Adds a subscription to the topic
    /// </summary>
    /// <param name="name">The subscription name.</param>
    /// <param name="sqlFilter">The SQL filter.</param>
    /// <returns></returns>
    public ServiceBusTopicBuilder AddSubscription(string name, string sqlFilter = null)
    {
        _topic.Subscriptions.Add(new ServiceBusSubscriptionModel
        {
            Name = name,
            SqlFilter = sqlFilter
        });
        return this;
    }

    internal ServiceBusTopicModel Build()
    {
        return _topic;
    }
}