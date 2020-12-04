using System;
using FluentAssertions;
using Xunit;

namespace Squadron
{
    public class GenericContainerResourceWithVolumeTests
        : IClassFixture<GenericContainerResource<SelemiumServerOptions>>
    {
        private readonly GenericContainerResource<SelemiumServerOptions> _resource;

        public GenericContainerResourceWithVolumeTests(
            GenericContainerResource<SelemiumServerOptions> resource)
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
    }

    public class SelemiumServerOptions : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder
                .Name("selenium-firefox")
                .InternalPort(4444)
                .ExternalPort(4444)
                .Image("selenium/standalone-firefox:latest")
                .AddVolume("/dev/shm:/dev/shm");
        }
    }
}
