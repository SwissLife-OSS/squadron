using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron;

public class GenericContainerWithEntrypointTests(ShellContainerResource resource)
    : IClassFixture<ShellContainerResource>
{
    [Fact]
    public async Task InvokeCommand_EntrypointKeepsContainerRunning_ReturnsOutput()
    {
        //Act
        string? output = await resource.InvokeAsync("echo", "hello");

        //Assert
        output.Should().Be("hello\n");
    }
}

public class ShellContainerResource : GenericContainerResource<ShellContainerOptions>
{
    public Task<string?> InvokeAsync(params string[] command)
    {
        return Manager.InvokeCommandAsync(command);
    }
}

public class ShellContainerOptions : GenericContainerOptions
{
    public override void Configure(ContainerResourceBuilder builder)
    {
        base.Configure(builder);
        // The default command of the image exits at once without a terminal.
        builder
            .Name("shell")
            .Image("alpine:3.20")
            .InternalPort(8080)
            .Entrypoint("tail", "-f", "/dev/null");
        ConfigureStatusChecker((_, _) => Task.FromResult(new Status { IsReady = true }));
    }
}
