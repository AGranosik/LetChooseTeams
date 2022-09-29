using LCT.Application.Common.Events;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace LCT.Application.Teams.Events
{
    public class TeamSelectedHubMessage: HubMessage
    {
        public override string Type { get => "TeamSelected"; }
        public Guid TournamentId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerSurname { get; set; }
        public string Team { get; set; }
    }

    public class TeamSelectedEventHandler : INotificationHandler<TeamSelectedDomainEvent>
    {
        private readonly IHubContext<TournamentHub> _hubContext;
        public TeamSelectedEventHandler(IHubContext<TournamentHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(TeamSelectedDomainEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _hubContext.Clients.All.SendCoreAsync(notification.StreamId.ToString(), new[] { 
                    new TeamSelectedHubMessage{
                        PlayerName = notification.Player.Name,
                        PlayerSurname = notification.Player.Surname,
                        Team = notification.TeamName,
                        TournamentId = notification.StreamId
                    }
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}
