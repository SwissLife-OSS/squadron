using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Neo4j.Driver;
using Snapshooter.Xunit;
using Squadron.Neo4j.Models;
using Xunit;
using DriverValue = Neo4j.Driver.ValueExtensions;

namespace Squadron
{
    public class Neo4jResourceTests
        : IClassFixture<Neo4jResource>
    {
        private Neo4jResource _neo4JResource { get; }

        public Neo4jResourceTests(Neo4jResource neo4jResource)
        {
            _neo4JResource = neo4jResource;
        }

        [Fact]
        public void GetSession_NoError()
        {
            //Act
            Action action = () => _neo4JResource.GetAsyncSession();

            //Assert
            action.Should().NotThrow();
        }

        [Fact]
        public async Task CreateActor()
        {
            // arrange
            IAsyncSession session = _neo4JResource.GetAsyncSession();

            try
            {
                // act
                var actor = new Actor("Keanu Reaves", 56);

                var parameters = new Dictionary<string, object>
                {
                    ["actorName"] = actor.Name,
                    ["actorAge"] = actor.Age
                };

                IResultCursor cursor = await session.RunAsync(
                    @"CREATE (actor:Actor {Name: $actorName, Age: $actorAge}) RETURN actor",
                    parameters);

                IRecord record = await cursor.SingleAsync();
                INode node = DriverValue.As<INode>(record["actor"]);
                
                Actor createdActor = new Actor(
                    DriverValue.As<string>(node.Properties["Name"]),
                    DriverValue.As<int>(node.Properties["Age"])
                );

                await cursor.ConsumeAsync();

                // assert
                createdActor.MatchSnapshot();
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
