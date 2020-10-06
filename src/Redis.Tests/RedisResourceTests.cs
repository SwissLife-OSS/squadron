using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using StackExchange.Redis;
using Xunit;

namespace Squadron
{
    public class RedisResourceTests : ISquadronResourceFixture<RedisResource>
    {
        private readonly RedisResource _redisResource;

        public RedisResourceTests(SquadronResource<RedisResource> redisResource)
        {
            _redisResource = redisResource.Resource;
        }


        [Fact]
        public void GetConnection_NoError()
        {
            //Act
            Action action = () =>
            {
                ConnectionMultiplexer redis = _redisResource.GetConnection();
                IDatabase db = redis.GetDatabase();
            };

            //Assert
            action.Should().NotThrow();
        }
    }
}
