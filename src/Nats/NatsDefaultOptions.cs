using System;

namespace Squadron;

/// <summary>
/// Default NATS resource options
/// </summary>
public class NatsDefaultOptions
    : ContainerResourceOptions
        , IComposableResourceOption
{
    public Type ResourceType => typeof(NatsResource);

    /// <summary>
    /// Configure resource options
    /// </summary>
    /// <param name="builder"></param>
    public override void Configure(ContainerResourceBuilder builder)
    {
        // 4222 is for clients.
        // 8222 is an HTTP management port for information reporting.
        // 6222 is a routing port for clustering (not used).

        builder
            .Name("nats")
            .Image("nats:latest")
            .AddVariable("nats-monitoring", VariableType.DynamicPort)
            .InternalPort(4222) // automatically dynamic because no use of ExternalPort
            .AddPortMapping(8222, "nats-monitoring")
            .PreferLocalImage();
    }
}