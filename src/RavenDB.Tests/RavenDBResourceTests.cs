using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Raven.Client.Documents.Session;
using Xunit;

namespace Squadron
{
    public class RavenDBResourceTests : IClassFixture<RavenDBResource>
    {
        private readonly RavenDBResource _ravenDBResource;

        public RavenDBResourceTests(RavenDBResource ravenDBResource)
        {
            _ravenDBResource = ravenDBResource;
        }

        [Fact]
        public void CreateDatabase_StoreUser_NoError()
        {
            //Act
            Action action = () =>
            {
                using (IDocumentSession session =
                    _ravenDBResource.CreateDatabase("test").OpenSession())
                {
                    session.Store(new User
                    {
                        Id = "1",
                        Name = "John"
                    });
                    session.SaveChanges();
                };
            };

            //Assert
            action.Should().NotThrow();
        }

        internal class User
        {
            public string Id { get; set; }

            public string Name { get; set; }
        }
    }
}
