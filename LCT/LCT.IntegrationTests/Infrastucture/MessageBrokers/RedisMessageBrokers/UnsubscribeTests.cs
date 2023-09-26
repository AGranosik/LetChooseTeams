using System.Threading.Tasks;
using FluentAssertions;
using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.MessageBrokers.Models;
using LCT.Infrastructure.MessageBrokers;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using System;

namespace LCT.IntegrationTests.Infrastucture.MessageBrokers.RedisMessageBrokers
{
    [TestFixture]
    internal class UnsubscribeTests
    {
        [Test]
        public async Task InvalidConnection_GroupIdIsNull_NoException()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);

            var action = () => rmb.UnsubscribeAsync(new MessageBrokerConnection(null, null));
            await action.Should().NotThrowAsync();

            subsriber.Invocations.Count.Should().Be(0);
            hubMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public async Task InvalidConnection_GroupIdIsEmpty_NoException()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);

            var action = () => rmb.UnsubscribeAsync(new MessageBrokerConnection(string.Empty, null));
            await action.Should().NotThrowAsync();

            subsriber.Invocations.Count.Should().Be(0);
            hubMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public async Task InvalidConnection_UserIdIsNull_NoException()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);

            var action = () => rmb.UnsubscribeAsync(new MessageBrokerConnection("groupId", null));
            await action.Should().NotThrowAsync();

            subsriber.Invocations.Count.Should().Be(0);
            hubMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public async Task InvalidConnection_UserIdIsEmpty_NoException()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);

            var action = () => rmb.UnsubscribeAsync(new MessageBrokerConnection("groupId", string.Empty));
            await action.Should().NotThrowAsync();

            subsriber.Invocations.Count.Should().Be(0);
            hubMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public async Task ConnectionDoesntExist_NotSubscribedBefore_NoException()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);

            var action = () => rmb.UnsubscribeAsync(new MessageBrokerConnection("groupId", "userId"));
            await action.Should().NotThrowAsync();

            subsriber.Invocations.Count.Should().Be(0);
            hubMock.Invocations.Count.Should().Be(0);
        }

        [Test]
        public async Task ConnectionExists_SubscribedBefore_UnsubscribeCalled()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);

            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var connection = new MessageBrokerConnection("groupId", "userId");

            await rmb.SubscribeAsync(connection);

            var action = () => rmb.UnsubscribeAsync(connection);
            await action.Should().NotThrowAsync();

            subsriber.Verify(s => s.UnsubscribeAsync(It.Is<RedisChannel>(c => c == connection.GroupId), It.IsAny<Action<RedisChannel, RedisValue>>(), It.Is<CommandFlags>(cf => cf == CommandFlags.None)), Times.Once());
        }

        [Test]
        public async Task MultipleConnectionsExist_ConnectionCannotBeClosed_UnsubscribeNotCalled()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);

            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var connection = new MessageBrokerConnection("groupId", "userId");

            await rmb.SubscribeAsync(connection);
            await rmb.SubscribeAsync(new MessageBrokerConnection("groupId", "userId2"));

            var action = () => rmb.UnsubscribeAsync(connection);
            await action.Should().NotThrowAsync();

            subsriber.Verify(s => s.UnsubscribeAsync(It.Is<RedisChannel>(c => c == connection.GroupId), It.IsAny<Action<RedisChannel, RedisValue>>(), It.Is<CommandFlags>(cf => cf == CommandFlags.None)), Times.Never());
        }

        [Test]
        public async Task MultipleConnectionsExist_ConnectionSHoulBeClosed_UnsubscribeCalled()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);

            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var connection = new MessageBrokerConnection("groupId", "userId");
            var connection2 = new MessageBrokerConnection("groupId", "userId2");

            await rmb.SubscribeAsync(connection);
            await rmb.SubscribeAsync(connection2);

            await rmb.UnsubscribeAsync(connection);
            await rmb.UnsubscribeAsync(connection2);

            subsriber.Verify(s => s.UnsubscribeAsync(It.Is<RedisChannel>(c => c == connection.GroupId), It.IsAny<Action<RedisChannel, RedisValue>>(), It.Is<CommandFlags>(cf => cf == CommandFlags.None)), Times.Once());
        }

    }
}
