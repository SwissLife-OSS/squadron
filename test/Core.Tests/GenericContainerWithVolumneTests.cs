using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron;

public class GenericContainerResourceWithVolumeTests(GenericContainerResource<NginxServerOptions> resource)
    : IClassFixture<GenericContainerResource<NginxServerOptions>>
{
    [Fact]
    public void PrepareResource_NoError()
    {
        //Act
        Action action = () =>
        {
            Uri externalUri = resource.GetContainerUri();
            Uri internalUri = resource.GetInternalContainerUri();
        };

        //Assert
        action.Should().NotThrow();
    }

    [Fact(Skip = "Temp")]
    public async Task PrepareResource_VolumeMapped()
    {
        //Arrange
        Uri externalUri = resource.GetContainerUri();
        using var client = new HttpClient();

        //Act
        using HttpResponseMessage response = await client.GetAsync(externalUri);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        //Assert
        content.Should().Contain("Hello Squadron unit test!");
    }
}

public class NginxServerOptions : GenericContainerOptions
{
    public override void Configure(ContainerResourceBuilder builder)
    {
        base.Configure(builder);
        builder
            .Name("nginx")
            .InternalPort(80)
            .ExternalPort(8811)
            .Image("nginx:latest")
            .AddVolume($"{Path.Combine(Directory.GetCurrentDirectory(),"test-volume")}:/usr/share/nginx/html");
    }
}