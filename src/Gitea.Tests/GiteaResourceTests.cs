using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace Squadron
{
    public class GiteaResourceTests : IClassFixture<GiteaResource>
    {
        private readonly GiteaResource _resource;

        public GiteaResourceTests(GiteaResource resource)
        {
            _resource = resource;
        }

        [Fact]
        public void PrepareResource_NoError()
        {
            //Act
            Action action = () =>
            {
                _resource.CreateApiClient();
            };

            //Assert
            action.Should().NotThrow();
        }
        
        [Fact]
        public async Task CreateRepository_Created()
        {
            // Arrange
            
            //Act
            var repo = await _resource.CreateRepositoryAsync("foo");
            
            //Assert
            repo.Should().NotBeNull();
            repo.Name.Should().Be("foo");
            repo.CloneUrl.Should().NotContain("3000");
            repo.Url.Should().NotContain("3000");
        }
    }
}
