using LCT.Application.Common.Events;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace LCT.Application.Players.Events
{
    public class PlayerAddedHubMessage : HubMessage
    {
        public override string Type => "PlayerAssigned";
        public Guid TournamentId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class PlayerAddedEventHandler : INotificationHandler<PlayerAddedDomainEvent>
    {
        private readonly IHubContext<TournamentHub> _hubContext;

        public PlayerAddedEventHandler(IHubContext<TournamentHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(PlayerAddedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _hubContext.Clients.All.SendCoreAsync(notification.StreamId.ToString(), new[] {
                new PlayerAddedHubMessage{
                    TournamentId = notification.StreamId,
                    Name  = notification.Name,
                    Surname = notification.Surname
                }
            }, cancellationToken);
        }
    }
}
