using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using LCT.Infrastructure.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace LCT.Application.Teams.Events
{
    public class TeamSelectedMessageEvent : EventMessage, INotification
    {
        public override string Type { get => "TeamSelected"; }
        public Guid TournamentId { get; set; }
        public string PlayerName { get; set; }
        public string PlayerSurname { get; set; }
        public string Team { get; set; }
    }

    public class TeamSelectedMessageEventHandler : INotificationHandler<TeamSelected>
    {
        private readonly IHubContext<PlayerAssignedHub> _hubContext;
        public TeamSelectedMessageEventHandler(IHubContext<PlayerAssignedHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(TeamSelected notification, CancellationToken cancellationToken)
        {
            try
            {
                await _hubContext.Clients.All.SendCoreAsync(notification.StreamId.ToString(), new[] { 
                    new TeamSelectedMessageEvent{
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
