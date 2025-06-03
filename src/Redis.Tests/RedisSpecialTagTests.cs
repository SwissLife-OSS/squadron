using System;
using FluentAssertions;
using StackExchange.Redis;
using Xunit;

namespace Squadron;

public class RedisSpecialTagTests(RedisResource<RedisSpecialTagOptions> redisResource)
    : IClassFixture<RedisResource<RedisSpecialTagOptions>>
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


public class RedisSpecialTagOptions : RedisDefaultOptions
{
    public override void Configure(ContainerResourceBuilder builder)
    {
        base.Configure(builder);
        builder.Tag("alpine");
    }
}