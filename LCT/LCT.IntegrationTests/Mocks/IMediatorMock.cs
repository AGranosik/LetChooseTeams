using LCT.Application.Players.Events;
using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Mocks
{
    public static class IMediatorMock
    {
        public static IMediator GetMock()
        {
            var mock = new Mock<IMediator>();
            mock.Setup(m => m.Publish(It.IsAny<object>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            
            return mock.Object;
        }

        public static IMediator GetMockWithException<TEvent>()
            where TEvent : INotification
        {
            var mock = new Mock<IMediator>();
            mock.Setup(m => m.Publish(It.IsAny<TEvent>(), It.IsAny<CancellationToken>())) //specific event
                .ThrowsAsync(new Exception());

            return mock.Object;
        }
    }
}
