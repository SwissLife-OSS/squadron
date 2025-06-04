using System;

namespace Squadron.AzureCloudEventHub.Tests;

public class TestExistingNamespaceAzureEventHubOptions : AzureCloudEventHubOptions
{
    public override void Configure(EventHubOptionsBuilder builder)
    {
        builder.Namespace("ehn-squadron-existing")
            .AddEventHub("testEventHub");
    }
}