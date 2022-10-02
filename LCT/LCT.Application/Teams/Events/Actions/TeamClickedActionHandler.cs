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
        public List<ClieckedPlayerTeam> ClickedTeams { get; set; }
    }

    public class ClieckedPlayerTeam
    {
        public string Team { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
    public class TeamClickedAction : LctAction<Guid>
    {
        public string Team { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
    public class TeamClickedActionHandler : INotificationHandler<TeamClickedAction>
    {
        private readonly ILctActionRepository<TeamClickedAction, Guid> _repository;
        private readonly IHubContext<TournamentHub> _hub;
        public TeamClickedActionHandler(ILctActionRepository<TeamClickedAction, Guid> repository, IHubContext<TournamentHub> hub)
        {
            _repository = repository;
            _hub = hub;
        }
        public async Task Handle(TeamClickedAction notification, CancellationToken cancellationToken)
        {
            var saved = await SaveAsync(notification);
            if (!saved)
                return;
            var allClickedTeams = await GetLatesClickedTeams(notification);

            await _hub.Clients.All.SendCoreAsync(notification.GroupKey.ToString(), new[]
            {
                allClickedTeams
            });
        }

        private async Task<TeamClickedEvent> GetLatesClickedTeams(TeamClickedAction action)
        {
            var actions = await _repository.GetByGroupIdAsync(action.GroupKey);
            var grouppedByPlayer = actions
                .GroupBy(a => new { a.Surname, a.Name })
                .Select(a => new ClieckedPlayerTeam
                {
                    Name = a.Key.Name,
                    Surname = a.Key.Surname,
                    Team = a.OrderByDescending(t => t.SavedTime).First().Team
                })
                .ToList();

            return new TeamClickedEvent
            {
                ClickedTeams = grouppedByPlayer,
                TournamentId = action.GroupKey
            };
        }

        private async Task<bool> SaveAsync(TeamClickedAction action)
        {
            if (IsTournamentNameEmpty(action.Team))
            {
                Log.Error("Team name cannot be empty in TeamSelectedAction");
                return false;
            }

            if (!IsTeamExist(action.Team))
            {
                Log.Error("Clicked team does not exist.");
                return false;
            }
            try
            {

            await _repository.SaveAsync(action);
            }
            catch(Exception ex)
            {

            }
            return true;
        }

        private bool IsTournamentNameEmpty(string teamName)
            => string.IsNullOrEmpty(teamName);

        private bool IsTeamExist(string teamName)
            => TournamentTeamNames.Teams.Any(t => teamName == t);

    }
}
