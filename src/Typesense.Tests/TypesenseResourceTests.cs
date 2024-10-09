using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Typesense;
using Typesense.Setup;
using Xunit;

namespace Squadron
{
    public class TypesenseResourceTests : IClassFixture<TypesenseResource>
    {
        private readonly TypesenseResource _typesenseResource;

        public TypesenseResourceTests(TypesenseResource typesenseResource)
        {
            _typesenseResource = typesenseResource;
        }

        [Fact]
        public void CreateAndGetDocument_NoError()
        {
            var provider = new ServiceCollection()
                .AddTypesenseClient(config =>
                {
                    config.ApiKey = _typesenseResource.ApiKey;
                    config.Nodes = new List<Node> { new Node("localhost", _typesenseResource.Port.ToString(), "http") };
                }, enableHttpCompression: false)
                .BuildServiceProvider();

            ITypesenseClient client = provider.GetRequiredService<ITypesenseClient>();

            //Act
            Action action = () =>
            {
                var schema = new Schema("users",
                        new[] { new Field("id", FieldType.String), 
                        new Field("name", FieldType.String) });
                
                client.CreateCollection(schema);
                client.CreateDocument("users", new User{ Id = "1", Name = "John Doe" });
                
                Task<User> _ = client.RetrieveDocument<User>("users", "1");
            };

            //Assert
            action.Should().NotThrow();
        }
    }

    internal class User
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}
