using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using StackExchange.Redis;
using Xunit;

namespace Squadron;

public class RedisResourceTests(RedisResource redisResource) : IClassFixture<RedisResource>
{
    [Fact]
    public void GetConnection_NoError()
    {
        //Act
        Action action = () =>
        {
            ConnectionMultiplexer redis = redisResource.GetConnection();
            IDatabase db = redis.GetDatabase();
        };

        //Assert
        action.Should().NotThrow();
    }
}