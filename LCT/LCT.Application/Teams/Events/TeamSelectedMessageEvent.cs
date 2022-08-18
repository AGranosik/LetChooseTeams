using LCT.Application.Tournaments.Hubs;
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

    public class TeamSelectedMessageEventHandler : INotificationHandler<TeamSelectedMessageEvent>
    {
        private readonly IHubContext<PlayerAssignedHub> _hubContext;
        public TeamSelectedMessageEventHandler(IHubContext<PlayerAssignedHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Handle(TeamSelectedMessageEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _hubContext.Clients.All.SendCoreAsync(notification.TournamentId.ToString(), new[] { notification }, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
        }
    }
}
