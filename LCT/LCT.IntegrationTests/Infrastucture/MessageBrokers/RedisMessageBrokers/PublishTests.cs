using System.Threading.Tasks;
using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.Infrastructure.MessageBrokers.Interfaces;
using LCT.Infrastructure.MessageBrokers;
using Microsoft.AspNetCore.SignalR;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using LCT.Infrastructure.MessageBrokers.Models;
using FluentAssertions;

namespace LCT.IntegrationTests.Infrastucture.MessageBrokers.RedisMessageBrokers
{
    [TestFixture]
    internal class PublishTests
    {
        [Test]
        public async Task MissingGroupId_PublishCalled_Success()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);
            redisConnectionMock.Setup(m => m.IsOpened())
                .Returns(true);


            var channel = "groupdId";
            var action = () => rmb.PublishAsync(channel, new MessageBrokerConnection("groupId", "userId"));
            await action.Should().NotThrowAsync();

            subsriber.Verify(s => s.PublishAsync(It.Is<RedisChannel>(c => c == channel), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()), Times.Once());
        }

        [Test]
        public async Task ConnectionIsOpened_PublishCalled_Success()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);
            redisConnectionMock.Setup(m => m.IsOpened())
                .Returns(true);

            var channel = "groupdId";
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId"));
            var action = () => rmb.PublishAsync(channel, new MessageBrokerConnection("groupId", "userId"));
            await action.Should().NotThrowAsync();

            subsriber.Verify(s => s.PublishAsync(It.Is<RedisChannel>(c => c == channel), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()), Times.Once());
        }

        [Test]
        public async Task ConnectionRestored_PublishCalledTwice_Success()
        {
            var redisConnectionMock = new Mock<IRedisConnection>();
            var hubMock = new Mock<IHubContext<TournamentHub>>();
            var rmb = new RedisMessageBroker(redisConnectionMock.Object, hubMock.Object);
            var subsriber = new Mock<ISubscriber>();

            redisConnectionMock.Setup(m => m.GetSubscriber())
                .ReturnsAsync(subsriber.Object);
            redisConnectionMock.Setup(m => m.IsOpened())
                .Returns(false);

            var channel = "groupdId";
            await rmb.SubscribeAsync(new MessageBrokerConnection(channel, "userId"));
            await rmb.PublishAsync(channel, new MessageBrokerConnection("groupId", "userId"));

            subsriber.Verify(s => s.PublishAsync(It.Is<RedisChannel>(c => c == channel), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()), Times.Never());

            redisConnectionMock.Setup(m => m.IsOpened())
                .Returns(true);

            await rmb.PublishAsync(channel, new MessageBrokerConnection("groupId", "userId"));

            subsriber.Verify(s => s.PublishAsync(It.Is<RedisChannel>(c => c == channel), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()), Times.Exactly(2));
        }
    }
}
