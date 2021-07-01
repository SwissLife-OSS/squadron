using System;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using FluentAssertions;
using Xunit;

namespace Squadron
{
    public class GenericContainerResourceWithMultiplePortsTests
        : IClassFixture<GenericContainerResource<MultiplePortsContainerOptions>>
    {
        private readonly GenericContainerResource<MultiplePortsContainerOptions> _resource;

        public GenericContainerResourceWithMultiplePortsTests(
            GenericContainerResource<MultiplePortsContainerOptions> resource)
        {
            _resource = resource;
        }

        [Fact]
        public async Task PrepareResource_NoError()
        {
            //Assert
            _resource.Instance.AdditionalPorts.Count.Should().Be(2);
            _resource.Instance.AdditionalPorts
                .Should()
                .ContainSingle(port => port.InternalPort == 29170);
            _resource.Instance.AdditionalPorts
                .Should()
                .ContainSingle(port => port.InternalPort == 29171);

            var client = new HttpClient();
            foreach (ContainerPortMapping port in _resource.Instance.AdditionalPorts)
            {
                HttpResponseMessage response = await client
                    .GetAsync($"http://{_resource.Address.Address}:{port.ExternalPort}");
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }
    }

    public class MultiplePortsContainerOptions : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder
                .Name("multi-port-test")
                .InternalPort(22)
                .AddPortMapping(29170)
                .AddPortMapping(29171)
                .Image("fredericbirke/test-image:latest");
        }
    }
}
