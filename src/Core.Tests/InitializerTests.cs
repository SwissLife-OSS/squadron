using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Squadron
{
    public class InitializerTests
    {

        [Fact]
        public async Task WaitAsync_NotReady_Throws()
        {
            //act
            Mock<IDockerContainerManager> managerMock = ArrangeContainerManagerMock();

            var initilizer = new ContainerInitializer(
                managerMock.Object,
                ContainerResourceBuilder.New()
                .WaitTimeout(3)
                .Build());

            // act
            Func<Task> action = async ()
                => await initilizer.WaitAsync(new NotReadyStatusProvider());

            // assert
            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task WaitAsync_Ready_IsReady()
        {
            //act
            Mock<IDockerContainerManager> managerMock = ArrangeContainerManagerMock();

            var initilizer = new ContainerInitializer(
                managerMock.Object,
                ContainerResourceBuilder.New()
                .WaitTimeout(7)
                .Build());

            // act
            Status result = await initilizer.WaitAsync(new OneThrowStatusProvider());


            // assert
            result.IsReady.Should().BeTrue();
        }

        private static Mock<IDockerContainerManager> ArrangeContainerManagerMock()
        {
            var mock = new Mock<IDockerContainerManager>(MockBehavior.Strict);
            mock.Setup(m => m.ConsumeLogsAsync(It.IsAny<TimeSpan>()))
                    .ReturnsAsync("Some Logs...");
            mock.SetupGet(p => p.Instance)
                    .Returns(new ContainerInstance { Logs = new List<string> { "Bang!" } });

            return mock;
        }

        private class NotReadyStatusProvider
            : IResourceStatusProvider
        {
            public Task<Status> IsReadyAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(Status.NotReady);
            }
        }

        private class OneThrowStatusProvider
            : IResourceStatusProvider
        {
            private int _runs;

            public Task<Status> IsReadyAsync(CancellationToken cancellationToken)
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
