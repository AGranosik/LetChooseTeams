using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Events;
using MediatR;
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
        private readonly IClientCommunicationService _clientCommunicationService;
        public TeamSelectedEventHandler(IClientCommunicationService clientCommunication)
        {
            _clientCommunicationService = clientCommunication;
        }
        public async Task Handle(TeamSelectedDomainEvent notification, CancellationToken cancellationToken)
        {
            await _clientCommunicationService.SendAsync(notification.StreamId.ToString(),
                new TeamSelectedHubMessage{
                    PlayerName = notification.Player.Name,
                    PlayerSurname = notification.Player.Surname,
                    Team = notification.TeamName,
                    TournamentId = notification.StreamId
            }, cancellationToken);
        }
    }
}
