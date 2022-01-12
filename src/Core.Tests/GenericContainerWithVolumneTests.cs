using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron
{
    public class GenericContainerResourceWithVolumeTests
        : IClassFixture<GenericContainerResource<NginxServerOptions>>
    {
        private readonly GenericContainerResource<NginxServerOptions> _resource;

        public GenericContainerResourceWithVolumeTests(
            GenericContainerResource<NginxServerOptions> resource)
        {
            _resource = resource;
        }

        [Fact]
        public void PrepareResource_NoError()
        {
            //Act
            Action action = () =>
            {
                Uri externalUri = _resource.GetContainerUri();
                Uri internalUri = _resource.GetInternalContainerUri();
            };

            //Assert
            action.Should().NotThrow();
        }

        [Fact]
        public async Task PrepareResource_VolumeMapped()
        {
            //Arrange
            Uri externalUri = _resource.GetContainerUri();
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
                .ExternalPort(8080)
                .Image("nginx:latest")
                .AddVolume($"{Path.Combine(Directory.GetCurrentDirectory(),"test-volume")}:/usr/share/nginx/html");
        }
    }
}
