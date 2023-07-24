using LCT.Application.Tournaments.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LCT.Application.Tournaments.Queries
{
    public class TestQuery : IRequest
    {
        public Guid Id { get; set; }   
    }
    internal class TestQueryHandler : IRequestHandler<TestQuery>
    {
        private readonly IHubContext<TournamentHub> _hubContext;
        public TestQueryHandler(IHubContext<TournamentHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task<Unit> Handle(TestQuery request, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendCoreAsync(request.Id.ToString(), new[]
            {
                "test"
            });

            return Unit.Value;
        }
    }
}
