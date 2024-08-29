using System;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron
{
    public class GenericContainerWithOnOptionsInitializedTests
    {
        private const string DefaultContainerName = "DefaultContainerName";

        [Fact]
        public async Task InitializeAsync_OptionsOverride_NameShouldBeOverridenValue()
        {
            // arrange
            const string newContainerName = "SomeName";
            var resource = new GenericNamedResource(newContainerName);

            // act
            await resource.InitializeAsync();

            // assert
            resource.Instance.Name.Should().Contain(newContainerName.ToLower());

            // cleanup
            await resource.DisposeAsync();
        }

        [Fact]
        public async Task InitializeAsync_WithoutOverride_NameShouldBeDefault()
        {
            // arrange
            var resource = new GenericNamedResource();

            // act
            await resource.InitializeAsync();

            // assert
            resource.Instance.Name.Should().Contain(DefaultContainerName.ToLower());

            // cleanup
            await resource.DisposeAsync();
        }

        class GenericNamedResource: GenericContainerResource<GenericNamedOptions>
        {
            private readonly string? _containerName;

            public GenericNamedResource(string? containerName = null)
            {
                _containerName = containerName;
            }

            protected override void OnOptionsInitialized(GenericNamedOptions options)
            {
                if (_containerName != null)
                {
                    options.SetContainerName(_containerName);
                }
            }
        }

        class GenericNamedOptions : GenericContainerOptions
        {
            private string _containerName = DefaultContainerName;

            public void SetContainerName(string containerName)
            {
                _containerName = containerName;
            }

            public override void Configure(ContainerResourceBuilder builder)
            {
                base.Configure(builder);
                builder
                    .Name(_containerName)
                    .Image("sickp/alpine-sshd:latest")
                    .InternalPort(22);
            }
        }
    }
}
