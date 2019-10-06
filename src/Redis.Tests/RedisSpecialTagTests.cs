using System;
using FluentAssertions;
using StackExchange.Redis;
using Xunit;

namespace Squadron
{
    public class RedisSpecialTagTests : IClassFixture<RedisResource<RedisSpecialTagOptions>>
    {
        private readonly RedisResource<RedisSpecialTagOptions> _redisResource;

        public RedisSpecialTagTests(RedisResource<RedisSpecialTagOptions> redisResource)
        {
            _redisResource = redisResource;
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


    public class RedisSpecialTagOptions : RedisDefaultOptions
    {
        public override void Configure(ContainerResourceBuilder builder)
        {
            base.Configure(builder);
            builder.Tag("alpine");
        }
    }

}
