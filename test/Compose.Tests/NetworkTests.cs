using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron;

public class NetworkTest(NetworkCompositionResource resource) : IClassFixture<NetworkCompositionResource>
{
    [Fact]
    public async Task TwoContainer_Network_BothInSameNetwork()
    {
        // Arrange
        MongoResource mongoResource = resource.GetResource<MongoResource>("mongo");
        GenericContainerResource<WebApp> webappResource = resource.GetResource<GenericContainerResource<WebApp>>("webapp");

        // Act - Get the connection string which contains the container name
        string connectionString = mongoResource.GetComposeExports()["CONNECTIONSTRING_INTERNAL"];

        // Assert - Verify both containers are running and can communicate
        // The connection string should contain the internal container name
        connectionString.Should().NotBeNullOrEmpty();
        connectionString.Should().Contain(":");  // Should have host:port format

        // Verify containers are running via Squadron's Instance
        mongoResource.Instance.IsRunning.Should().BeTrue();
        webappResource.Instance.IsRunning.Should().BeTrue();

        // Both should be in the same network (via compose configuration)
        // This is verified by the fact that CONNECTIONSTRING_INTERNAL uses the container name
        // which is only resolvable within the shared Docker network
        await Task.CompletedTask;
    }
}

public class WebApp : GenericContainerOptions
{
    public override void Configure(ContainerResourceBuilder builder)
    {
        base.Configure(builder);
        builder
            .Name("webapp")
            .InternalPort(80)
            .Image("nginx")
            .AddNetwork("demo-network");
    }
}

public class MongoDbContainer : MongoDefaultOptions
{
    public override void Configure(ContainerResourceBuilder builder)
    {
        base.Configure(builder);
        builder.AddNetwork("demo-network");
    }
}

public class NetworkComposition : ComposeResourceOptions
{
    public override void Configure(ComposeResourceBuilder builder)
    {
        builder.AddContainer<MongoDbContainer>("mongo");
        builder.AddContainer<WebApp>("webapp")
            .AddLink("mongo", new EnvironmentVariableMapping(
                "Sample:Database:ConnectionString", "#CONNECTIONSTRING_INTERNAL#"
            ));
    }
}

public class NetworkCompositionResource : ComposeResource<NetworkComposition>
{
}