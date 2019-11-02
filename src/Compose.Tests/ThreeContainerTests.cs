using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron
{
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
    }


    public class ThreeContainerOptions : ComposeResourceOptions
    {
        public override void Configure(ComposeResourceBuilder builder)
        {
            builder.AddContainer<MongoDefaultOptions>("mongo");
            builder.AddContainer<TestWebServerOptions>("api")
                    .AddLink("mongo", new EnvironmentVariableMapping(
                                "UserManagement:Database:ConnectionString",
                                "#CONNECTIONSTRING#"
                                ));

            builder.AddContainer<TestWebServerOptions>("ui")
                    .AddLink("api", new EnvironmentVariableMapping(
                                "UserManagament:Host", "#HTTPURL#"
                                ));
        }
    }

    public class TwoContainerComposedResource : ComposeResource<ThreeContainerOptions>
    {


    }


    public class ThreeContainerTests : IClassFixture<TwoContainerComposedResource>
    {
        private readonly TwoContainerComposedResource _resource;

        public ThreeContainerTests(TwoContainerComposedResource resource)
        {
            _resource = resource;
        }

        [Fact]
        public void ThreeContainer_NoError()
        {
            //Act
            Action action = () =>
            {
                MongoResource mongo = _resource.GetResource<MongoResource>("mongo");
            };

            //Assert
            action.Should().NotThrow();
        }

    }
}
