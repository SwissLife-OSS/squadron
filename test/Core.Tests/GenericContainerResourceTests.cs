using System;
using FluentAssertions;
using Xunit;

namespace Squadron;

public class GenericContainerResourceTests
    // Uncomment the following line to run the test
    // : IClassFixture<GenericContainerResource<TestWebServerOptions>>
    (GenericContainerResource<TestWebServerOptions> resource)
{
    [Fact(Skip = "Can not run without registry credentials")]
    public void PrepareResource_NoError()
    {
        //Act
        Action action = () =>
        {
            resource.GetContainerUri();
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
            .ExternalPort(4200)
            .Image("spcasquadron.azurecr.io/fusion-login-samples:v2")
            .Registry("myPrivate");
    }
}