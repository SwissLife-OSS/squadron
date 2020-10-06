using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace Squadron
{
    internal class LocalAppOptions : GenericContainerOptions
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
            await Helpers.GetDockerClient.Images.CreateImageAsync(
                new ImagesCreateParameters
                {
                    FromImage = "nginx:latest",
                },
                null,
                new Progress<JSONMessage>(Handler),
                CancellationToken.None);

            // Re-tagging the Nginx image to our test name
            await Helpers.GetDockerClient.Images.TagImageAsync(
                "nginx:latest",
                new ImageTagParameters
                {
                    Tag = TestData.LocalTagVersion,
                    RepositoryName = TestData.LocalTagName
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
                .Image(TestData.LocalTagName)
                .Tag(TestData.LocalTagVersion)
                .PreferLocalImage();
        }
    }
}
