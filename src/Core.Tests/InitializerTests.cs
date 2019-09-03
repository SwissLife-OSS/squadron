using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Squadron
{
    public class InitializerTests
    {
        [Fact]
        public async Task GivenNotReadyStatus_WhenWaitToInitialize_Throws()
        {
            // arrange
            Task initializerTask = Initializer.WaitAsync(
                new NotReadyStatusProvider(),
                TimeSpan.FromSeconds(5), Mock.Of<IImageSettings>());
            Task timeout = Task.Delay(TimeSpan.FromSeconds(10));

            // act
            // assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await Task
                    .WhenAny(initializerTask, timeout)
                    .Unwrap();
            });
        }

        [Fact]
        public async Task GivenReadyStatusWithOneThrow_WhenWaitToInitialize_IsReady()
        {
            // arrange
            Task<Status> initializerTask = Initializer.WaitAsync(
                new OneThrowStatusProvider(),
                TimeSpan.FromSeconds(5), Mock.Of<IImageSettings>());
            Task timeout = Task.Delay(TimeSpan.FromSeconds(10));

            // act
            Task result = await Task
                .WhenAny(initializerTask, timeout);

            // assert
            Assert.Equal(result, initializerTask);
            Status status = await initializerTask;
            Assert.True(status.IsReady);
        }

        private class NotReadyStatusProvider
            : IResourceStatusProvider
        {
            public Task<Status> IsReadyAsync()
            {
                return Task.FromResult(Status.NotReady);
            }
        }

        private class OneThrowStatusProvider
            : IResourceStatusProvider
        {
            private int _runs;

            public Task<Status> IsReadyAsync()
            {
                if (_runs <= 0)
                {
                    _runs++;
                    throw new TimeoutException();
                }

                return Task.FromResult(new Status()
                {
                    IsReady = true
                });
            }
        }
    }
}
