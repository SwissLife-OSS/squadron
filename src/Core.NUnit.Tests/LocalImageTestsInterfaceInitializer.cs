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
        NUnitSquadronInterfaceInitializer,
        IResourceFixture<GenericContainerResource<LocalAppOptions>>
    {
        public static string LocalTagName { get; } = "test-image";
        public static string LocalTagVersion { get; } = "1.0.0";

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
                GetResource<GenericContainerResource<LocalAppOptions>>();

            // Act
            Uri containerUri = resource.GetContainerUri();

            // Assert
            containerUri.Should().NotBeNull();

            await DockerClient.Images.DeleteImageAsync(
                $"{LocalTagName}:{LocalTagVersion}",
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
