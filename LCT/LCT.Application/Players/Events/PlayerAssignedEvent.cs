using LCT.Application.Tournaments.Hubs;
using LCT.Infrastructure.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace LCT.Application.Players.Events
{
    public class PlayerAssignedEvent : EventMessage, INotification
    {
        public override string Type => "PlayerAssigned";
        public Guid TournamentId { get; set; }
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }

    public class PlayerAssignedEventHandler : INotificationHandler<PlayerAssignedEvent>
    {
        private readonly IHubContext<PlayerAssignedHub> _hubContext;

        public PlayerAssignedEventHandler(IHubContext<PlayerAssignedHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(PlayerAssignedEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                await _hubContext.Clients.All.SendCoreAsync(notification.TournamentId.ToString(), new[] { notification }, cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception during signalr conntection");
            }
        }
    }
}
