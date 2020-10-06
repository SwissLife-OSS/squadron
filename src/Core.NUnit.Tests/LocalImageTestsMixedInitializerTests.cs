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
    public class LocalImageTestsMixedInitializerTests :
        SquadronInitializer, ISquadronResourceFixture<GenericContainerResource<LocalAppOptions>>
    {
        [NUnitSquadronInject]
        private GenericContainerResource<LocalAppOptions> _resource;

        [Test]
        public void IsResourcePropertyInitializedTest()
        {
            // Arrange: See LocalAppOptions
            // Act
            // Assert
            _resource.Should().NotBeNull();
        }

        [Test]
        public void IsResourceListInitialized()
        {
            // Arrange: See LocalAppOptions
            // Act
            GenericContainerResource<LocalAppOptions> resource =
                GetSquadronResource<GenericContainerResource<LocalAppOptions>>();

            // Assert
            resource.Should().NotBeNull();
        }
    }
}
