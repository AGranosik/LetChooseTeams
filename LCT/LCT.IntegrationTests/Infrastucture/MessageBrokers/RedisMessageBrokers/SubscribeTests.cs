using System;
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
        public async Task Subscriber_SubscribeCalled_Success()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);
            var channel = "groupId";

            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var action = () => rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId"));
            await action.Should().NotThrowAsync();
            subsriber.Verify(s => s.SubscribeAsync(It.Is<RedisChannel>(c => c == channel),It.IsAny<Action<RedisChannel, RedisValue>>(), It.Is<CommandFlags>(cf => cf == CommandFlags.None)), Times.Once());

        }

        [Test]
        public async Task Subscriber_ConnectionAlreadyOpened_SubsribeCalledOnlyOnce()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);
            var channel = "groupId";

            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId"));
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId"));
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId3"));
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId4"));

            subsriber.Verify(s => s.SubscribeAsync(It.Is<RedisChannel>(c => c == channel), It.IsAny<Action<RedisChannel, RedisValue>>(), It.Is<CommandFlags>(cf => cf == CommandFlags.None)), Times.Once());
        }

        [Test]
        public async Task Subscriber_ConnectionClosed_SubsribeCalledTwice()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);
            var channel = "groupId";

            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId"));
            await rmb.UnsubscribeAsync(new MessageBrokerConnection(channel, "userId"));
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId"));

            subsriber.Verify(s => s.SubscribeAsync(It.Is<RedisChannel>(c => c == channel), It.IsAny<Action<RedisChannel, RedisValue>>(), It.Is<CommandFlags>(cf => cf == CommandFlags.None)), Times.Exactly(2));
        }

        [Test]
        public async Task Subscriber_ConnectionStayOpen_SubsribeCalledOnce()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);
            var channel = "groupId";

            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId"));
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId2"));
            await rmb.UnsubscribeAsync(new MessageBrokerConnection(channel, "userId"));
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId3"));

            subsriber.Verify(s => s.SubscribeAsync(It.Is<RedisChannel>(c => c == channel), It.IsAny<Action<RedisChannel, RedisValue>>(), It.Is<CommandFlags>(cf => cf == CommandFlags.None)), Times.Once());
        }
    }
}
