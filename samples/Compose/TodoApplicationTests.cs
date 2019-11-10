using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron.Samples.Compose
{
    public class FrontEndAppOptions : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder
                .Name("frontend")
                .InternalPort(80)
                .Image("nginx");
        }
    }

    public class GraphQLApiOptions : GenericContainerOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder
                .Name("graphql")
                .InternalPort(80)
                .Image("nginx");
        }
    }


    public class MongoDb : MongoDefaultOptions
    { }


    public class TodoAppOptions : ComposeResourceOptions
    {
        public override void Configure(ComposeResourceBuilder builder)
        {
            builder.AddContainer<MongoDb>("db");
            builder.AddContainer<GraphQLApiOptions>("api")
                    .AddLink("db", new EnvironmentVariableMapping(
                                "Sample:Database:ConnectionString",
                                "#CONNECTIONSTRING#"
                                ));

            builder.AddContainer<FrontEndAppOptions>("ui")
                    .AddLink("api", new EnvironmentVariableMapping(
                                "Sample:Api:Url", "#HTTPURL#"
                                ));
        }
    }


    public class TodoApplicationTests : IClassFixture<ComposeResource<TodoAppOptions>>
    {
        private readonly ComposeResource<TodoAppOptions> _composeResource;

        public TodoApplicationTests(ComposeResource<TodoAppOptions> composeResource)
        {
            _composeResource = composeResource;
        }

        [Fact]
        public async Task OpenFrontend_Ok()
        {
            GenericContainerResource<FrontEndAppOptions> frontEnd = _composeResource
                .GetResource<GenericContainerResource<FrontEndAppOptions>>("ui");

            var client = new HttpClient
            {
                BaseAddress = frontEnd.GetContainerUri()
            };

            HttpResponseMessage frontResponse = await client.GetAsync("/");
            frontResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }
}
