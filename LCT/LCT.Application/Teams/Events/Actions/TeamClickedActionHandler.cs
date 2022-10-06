using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Application.Tournaments.Hubs;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Common.Interfaces;
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
        private readonly IAggregateRepository<Tournament> _aggregateRepository;
        public TeamClickedActionHandler(ILctActionRepository<TeamClickedAction, Guid> repository, IHubContext<TournamentHub> hub, IAggregateRepository<Tournament> aggregateRepository)
        {
            _repository = repository;
            _hub = hub;
            _aggregateRepository = aggregateRepository;
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
            Tournament tournament = new Tournament();
            try
            {
                tournament = await _aggregateRepository.LoadAsync(action.GroupKey);
            }
            catch(Exception ex) { }

            var playerPickedTeams = tournament.SelectedTeams.Select(st => st.Player);
            var actions = await _repository.GetByGroupIdAsync(action.GroupKey);
            var grouppedByPlayer = actions
                .Where(a => !playerPickedTeams.Any(p => p.Name == a.Name && p.Surname == a.Surname))
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
            if (!IsTeamExist(action.Team))
            {
                Log.Error("Clicked team does not exist.");
                return false;
            }
            await _repository.SaveAsync(action);
            return true;
        }

        private bool IsTeamNameEmpty(string teamName)
            => string.IsNullOrEmpty(teamName);

        private bool IsTeamExist(string teamName)
            => TournamentTeamNames.Teams.Any(t => teamName == t);

    }
}
