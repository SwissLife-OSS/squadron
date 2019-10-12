using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron
{
    public class GenericContainerResourceTests
        : IClassFixture<GenericContainerResource<TestWebServerOptions>>
    {
        private readonly GenericContainerResource<TestWebServerOptions> _resource;

        public GenericContainerResourceTests(GenericContainerResource<TestWebServerOptions> resource)
        {
            _resource = resource;
        }

        [Fact(Skip = "Can not run without registry credentials")]

        public void PrepareResource_NoError()
        {
            //Act
            Action action = () =>
            {
                _resource.GetContainerUri();
            };

            //Assert
            action.Should().NotThrow();
        }

    }

    public class TestWebServerOptions : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder
                .Name("login-samples")
                .InternalPort(4200)
                .Image("spcasquadron.azurecr.io/fusion-login-samples:v2")
                .Registry("myPrivate");
        }

        public async Task<Status> IsReadyAsync(ContainerAddress containerAddress, CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage res = await httpClient
                .GetAsync($"http://{containerAddress.Address}:{containerAddress.Port}");

            return new Status
            {
                IsReady = true,
                Message = $"{res.StatusCode} -> {res.RequestMessage.RequestUri}"
            };
        }
    }
}
