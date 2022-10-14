using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.AspNetCore.SignalR;
using Moq;

namespace LCT.IntegrationTests.Mocks
{
    public static class IHubContextMock
    {
        public static IHubContext<T> GetMockedHubContext<T>(Mock<IHubClients> hubClients = null, Mock<IClientProxy> clientProxy = null, Expression<Func<object?[], bool>> verify = null)
            where T : Hub
        {
            var hubContext = new Mock<IHubContext<T>>();
            hubClients ??= new Mock<IHubClients>();
            clientProxy ??= new Mock<IClientProxy>();
            if(verify is not null)
            {
                clientProxy.Verify(c => c.SendCoreAsync(It.IsAny<string>(), It.Is<object?[]>(verify), CancellationToken.None));
            }
            hubClients.Setup(hc => hc.All)
                .Returns(clientProxy.Object);


            hubContext.Setup(hc => hc.Clients)
                .Returns(hubClients.Object);

            return hubContext.Object;
        }
    }
}
