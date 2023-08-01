namespace LCT.Application.Common.Interfaces
{
    public interface IClientCommunicationService
    {
        Task SendAsync<T>(string destination, T message, CancellationToken cancellationToken)
            where T : class;
    }
}
