using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Squadron
{
    public class LocalImageTestsManualInitializationTests
    {
        private GenericContainerResource<LocalAppOptions> _resource;

        public LocalImageTestsManualInitializationTests()
        {
            _resource = new GenericContainerResource<LocalAppOptions>();
        }

        [OneTimeSetUp]
        protected async Task OneTimeSetUp()
        {
            await _resource.InitializeAsync();
        }

        [OneTimeTearDown]
        protected async Task OneTimeTearDown()
        {
            await _resource.DisposeAsync();
        }

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
