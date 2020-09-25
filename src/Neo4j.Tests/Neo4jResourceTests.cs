using System;
using FluentAssertions;
using Neo4j.Driver;
using Xunit;
using Squadron.Neo4j.Models;
using Neo4jMapper;
using System.Collections.Generic;
using Snapshooter.Xunit;

namespace Squadron {
    public class Neo4jResourceTests
        : IClassFixture<Neo4jResource> {
            private Neo4jResource _neo4JResource { get; }

            public Neo4jResourceTests (Neo4jResource neo4jResource) {
                _neo4JResource = neo4jResource;
            }

            [Fact]
            public void GetSession_NoError () {
                //Act
                Action action = () => _neo4JResource.GetAsyncSession ();

                //Assert
                action.Should ().NotThrow ();
            }

        [Fact]
        public async void CreateActor()
        {
            IAsyncSession session = _neo4JResource.GetAsyncSession();

            try
            {
                var actor = new Actor("Keanu Reaves", 43);

                IDictionary<string, object> parameters = new Neo4jParameters().WithEntity("newActor", actor);

                IResultCursor cursor = await session.RunAsync(@"CREATE (actor:Actor $newActor) RETURN actor", parameters);

                Actor createdMovie = await cursor.MapSingleAsync<Actor>();

                await cursor.ConsumeAsync();

                actor.MatchSnapshot();
            }
            finally
            {
                await session.CloseAsync();
            }
        }
    }
}
