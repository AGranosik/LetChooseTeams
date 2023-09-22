using System.Threading.Tasks;
using FluentAssertions;
using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.Infrastructure.MessageBrokers;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.MessageBrokers.Models;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace LCT.IntegrationTests.Infrastucture.MessageBrokers.RedisMessageBrokers
{
    [TestFixture]
    internal class SubscribeTests
    {
        [Test]
        public async Task InvalidConnection_GroupIdIsNull_NoException()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);

            var action = () => rmb.SubscribeAsync(new MessageBrokerConnection(null, null));
            await action.Should().NotThrowAsync();

            redisConnectionMock.Invocations.Count.Should().Be(0);
            hubMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public async Task InvalidConnection_GroupIdIsEmpty_NoException()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);

            var action = () => rmb.SubscribeAsync(new MessageBrokerConnection(string.Empty, null));
            await action.Should().NotThrowAsync();

            redisConnectionMock.Invocations.Count.Should().Be(0);
            hubMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public async Task InvalidConnection_UserIdIsNull_NoException()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);

            var action = () => rmb.SubscribeAsync(new MessageBrokerConnection("groupId", null));
            await action.Should().NotThrowAsync();

            redisConnectionMock.Invocations.Count.Should().Be(0);
            hubMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public async Task InvalidConnection_UserIdIsEmpty_NoException()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);

            var action = () => rmb.SubscribeAsync(new MessageBrokerConnection("groupId", string.Empty));
            await action.Should().NotThrowAsync();

            redisConnectionMock.Invocations.Count.Should().Be(0);
            hubMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public async Task InvalidSubscriber_SubscribeNotCalled_Success()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(default(ISubscriber));

            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var action = 

        }
    }
}
