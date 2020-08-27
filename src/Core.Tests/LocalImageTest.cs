using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using FluentAssertions;
using Xunit;

namespace Squadron
{
    public class LocalImageTest : IClassFixture<GenericContainerResource<LocalAppOptions>>
    {
        public static string LocalTagName { get; } = "test-image";
        public static string LocalTagVersion { get; } = "1.0.0";

        public static DockerClient DockerClient =
            new DockerClientConfiguration(
                LocalDockerUri(),
                null,
                TimeSpan.FromMinutes(5))
            .CreateClient();

        private GenericContainerResource<LocalAppOptions> _containerResource;

        public LocalImageTest(GenericContainerResource<LocalAppOptions> containerResource)
        {
            _containerResource = containerResource;
        }

        [Fact]
        public async Task UseLocalImageTest()
        {
            Uri containerUri = _containerResource.GetContainerUri();
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

    public class LocalAppOptions : GenericContainerOptions
    {
        public async Task CreateAndTagImage()
        {
            void Handler(JSONMessage message)
            {
                if (!string.IsNullOrEmpty(message.ErrorMessage))
                {
                    throw new ContainerException(
                        $"Error: {message.ErrorMessage}");
                }
            }

            // Pulling Nginx image
            await LocalImageTest.DockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = "nginx:latest",
                },
                null,
                new Progress<JSONMessage>(Handler),
                CancellationToken.None);

            // Re-tagging the Nginx image to our test name
            await LocalImageTest.DockerClient.Images.TagImageAsync(
                "nginx:latest",
                new ImageTagParameters
                {
                    Tag = LocalImageTest.LocalTagVersion,
                    RepositoryName = LocalImageTest.LocalTagName
                },
                CancellationToken.None);
        }

        public override void Configure(ContainerResourceBuilder builder)
        {
            CreateAndTagImage().Wait();

            base.Configure(builder);
            builder
                .Name("local-demo-image")
                .InternalPort(80)
                .Image(LocalImageTest.LocalTagName)
                .Tag(LocalImageTest.LocalTagVersion)
                .PreferLocalImage(true);
        }
    }
}
