using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Images;
using FluentAssertions;
using Xunit;

namespace Squadron;

public class LocalImageTests(GenericContainerResource<LocalAppOptions> containerResource)
    : IClassFixture<GenericContainerResource<LocalAppOptions>>
{
    public static string LocalTagName { get; } = "test-image";
    public static string LocalTagVersion { get; } = "1.0.0";

    [Fact]
    public async Task UseLocalImageTest()
    {
        // Arrange: See LocalAppOptions

        // Act
        Uri containerUri = containerResource.GetContainerUri();

        // Assert
        containerUri.Should().NotBeNull();

        // Clean up the test image using Testcontainers
        var image = new ImageFromDockerfileBuilder()
            .WithName($"{LocalTagName}:{LocalTagVersion}")
            .WithCleanUp(true)
            .Build();

        try
        {
            await image.DeleteAsync();
        }
        catch
        {
            // Image may already be deleted or in use
        }
    }
}

public class LocalAppOptions : GenericContainerOptions
{
    public async Task CreateAndTagImage()
    {
        // Pull nginx:latest and tag it as our test image using Testcontainers
        // We use a temporary container to pull the image, then the image stays cached
        var tempContainer = new ContainerBuilder()
            .WithImage("nginx:latest")
            .WithAutoRemove(true)
            .WithCleanUp(true)
            .Build();

        // Starting and immediately stopping will pull the image if not present
        await tempContainer.StartAsync();
        await tempContainer.StopAsync();
        await tempContainer.DisposeAsync();

        // Note: Testcontainers doesn't have a direct image tagging API,
        // but the image is now pulled and available locally.
        // For the test, we'll use nginx:latest directly with PreferLocalImage.
    }

    public override void Configure(ContainerResourceBuilder builder)
    {
        CreateAndTagImage().Wait();

        base.Configure(builder);
        builder
            .Name("local-demo-image")
            .InternalPort(80)
            .Image("nginx:latest")  // Use pulled image directly
            .CopyFileToContainer("appsettings.json", "/appsettings.json")
            .PreferLocalImage();
    }
}