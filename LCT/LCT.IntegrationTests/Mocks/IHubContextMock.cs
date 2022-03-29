using Microsoft.AspNetCore.SignalR;
using Moq;

namespace LCT.IntegrationTests.Mocks
{
    public static class IHubContextMock
    {
        public static IHubContext<T> GetMockedHubContext<T>(Mock<IHubClients> hubClients = null, Mock<IClientProxy> clientProxy = null)
            where T : Hub
        {
            var hubContext = new Mock<IHubContext<T>>();
            hubClients = hubClients ?? new Mock<IHubClients>();
            clientProxy = clientProxy ?? new Mock<IClientProxy>();
            hubClients.Setup(hc => hc.All)
                .Returns(clientProxy.Object);

            hubContext.Setup(hc => hc.Clients)
                .Returns(hubClients.Object);

            return hubContext.Object;
        }
    }
}
