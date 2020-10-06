using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using NUnit.Framework;


namespace Squadron
{
    public class LocalImageTestsInterfaceInitializerTests :
        SquadronInitializer,
        ISquadronResourceFixture<GenericContainerResource<LocalAppOptions>>
    {
        [Test]
        public async Task UseLocalImageTest()
        {
            // Arrange: See LocalAppOptions
            GenericContainerResource<LocalAppOptions> resource =
                GetSquadronResource<GenericContainerResource<LocalAppOptions>>();

            // Act
            Uri containerUri = resource.GetContainerUri();

            // Assert
            containerUri.Should().NotBeNull();

            await Helpers.GetDockerClient.Images.DeleteImageAsync(
                $"{TestData.LocalTagName}:{TestData.LocalTagVersion}",
                new ImageDeleteParameters(),
                CancellationToken.None);
        }
    }
}
