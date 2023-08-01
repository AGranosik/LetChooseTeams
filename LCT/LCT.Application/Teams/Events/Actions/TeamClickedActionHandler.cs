using LCT.Application.Common.Events;
using LCT.Application.Common.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Common.Interfaces;
using MediatR;
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
        private readonly IClientCommunicationService _clientCommunicationService;
        private readonly IAggregateRepository<Tournament> _aggregateRepository;
        public TeamClickedActionHandler(ILctActionRepository<TeamClickedAction, Guid> repository, IClientCommunicationService clientCommunicationService, IAggregateRepository<Tournament> aggregateRepository)
        {
            _repository = repository;
            _clientCommunicationService = clientCommunicationService;
            _aggregateRepository = aggregateRepository;
        }
        public async Task Handle(TeamClickedAction notification, CancellationToken cancellationToken)
        {
            var saved = await SaveAsync(notification);
            if (!saved)
                return;
            var allClickedTeams = await GetLatesClickedTeams(notification);
            await _clientCommunicationService.SendAsync(notification.GroupKey.ToString(), new[]
            {
                allClickedTeams
            }, cancellationToken);
        }

        private async Task<TeamClickedEvent> GetLatesClickedTeams(TeamClickedAction action)
        {
            Tournament tournament = new();
            try
            {
                tournament = await _aggregateRepository.LoadAsync(action.GroupKey);
                // the should not be try catch
                // 
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

        private bool IsTeamExist(string teamName)
            => TournamentTeamNames.Teams.Any(t => teamName == t);

    }
}
