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
    public class LocalImageTestsPropertyInitializerTests : SquadronInitializer
    {
        [NUnitSquadronInject]
        private GenericContainerResource<LocalAppOptions> _resource;

        [Test]
        public async Task UseLocalImageTest()
        {
            // Arrange: See LocalAppOptions
            // Act
            Uri containerUri = _resource.GetContainerUri();

            // Assert
            containerUri.Should().NotBeNull();

            await Helpers.GetDockerClient.Images.DeleteImageAsync(
                $"{TestData.LocalTagName}:{TestData.LocalTagVersion}",
                new ImageDeleteParameters(),
                CancellationToken.None);
        }
    }
}
