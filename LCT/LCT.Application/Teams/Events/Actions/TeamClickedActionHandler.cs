using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace LCT.Application.Teams.Events.Actions
{
    public class TeamClickedEvent : HubMessage
    {
        public override string Type => "TeamClicked";
        public Guid TournamentId { get; set; }
        public string Team { get; set; }
    }
    public class TeamClickedAction : LctAction<Guid>
    {
        public string Team { get; set; }
    }
    public class TeamClickedActionHandler : INotificationHandler<TeamClickedAction>
    {
        private readonly ILctActionRepository<TeamClickedAction> _repository;
        private readonly IHubContext<TournamentHub> _hub;
        public TeamClickedActionHandler(ILctActionRepository<TeamClickedAction> repository, IHubContext<TournamentHub> hub)
        {
            _repository = repository;
            _hub = hub;
        }
        public async Task Handle(TeamClickedAction notification, CancellationToken cancellationToken)
        {
            if (IsTournamentNameEmpty(notification.Team))
            {
                Log.Error("Team name cannot be empty in TeamSelectedAction");
                return;
            }

            if (!IsTeamExist(notification.Team))
            {
                Log.Error("Clicked team does not exist.");
                return;
            }

            await _repository.SaveAsync(notification);
            await _hub.Clients.All.SendCoreAsync(notification.GroupKey.ToString(), new[]
            {
                new TeamClickedEvent
                {
                    Team = notification.Team,
                    TournamentId = notification.GroupKey
                }
            });
        }

        private bool IsTournamentNameEmpty(string teamName)
            => string.IsNullOrEmpty(teamName);

        private bool IsTeamExist(string teamName)
            => TournamentTeamNames.Teams.Any(t => teamName == t);

    }
}
