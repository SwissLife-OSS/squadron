using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Squadron;
using StackExchange.Redis;
using Xunit;

namespace Redis.Tests
{
    public class RedisResourceTests : IClassFixture<RedisResource>
    {
        private readonly RedisResource _redisResource;

        public RedisResourceTests(RedisResource redisResource)
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

        [Fact]
        public async Task GetConnectionAsync_NoError()
        {
            //Act
            Func<Task> action = async () =>
            {
                ConnectionMultiplexer redis = await
                    _redisResource.GetConnectionAsync();
                IDatabase db = redis.GetDatabase();
            };

            //Assert
            await action.Should().NotThrowAsync();
        }

    }
}
