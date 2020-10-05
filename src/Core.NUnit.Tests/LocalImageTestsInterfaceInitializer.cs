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
    public class LocalImageTestsInterfaceInitializer :
        SquadronInitializer,
        ISquadronResourceFixture<GenericContainerResource<LocalAppOptions>>
    {
        public static DockerClient DockerClient =
            new DockerClientConfiguration(
                LocalDockerUri(),
                null,
                TimeSpan.FromMinutes(5))
            .CreateClient();

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

            await DockerClient.Images.DeleteImageAsync(
                $"{TestData.LocalTagName}:{TestData.LocalTagVersion}",
                new ImageDeleteParameters(),
                CancellationToken.None);
        }

        private static Uri LocalDockerUri()
        {
#if NET461
            return new Uri("npipe://./pipe/docker_engine");
#else
            var isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            return isWindows ?
                new Uri("npipe://./pipe/docker_engine") :
                new Uri("unix:/var/run/docker.sock");
#endif
        }
    }
}
