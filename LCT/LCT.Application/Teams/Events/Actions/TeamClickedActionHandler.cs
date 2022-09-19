using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Infrastructure.Events;
using LCT.Infrastructure.Repositories.Actions;
using MediatR;
using Serilog;

namespace LCT.Application.Teams.Events.Actions
{
    public class TeamClickedAction : LctAction<Guid>
    {
        public string Team { get; set; }
    }
    public class TeamSelectedActionHandler : INotificationHandler<TeamClickedAction>
    {
        private readonly ILctActionRepository<TeamClickedAction> _repository;
        public TeamSelectedActionHandler(ILctActionRepository<TeamClickedAction> repository)
        {
            _repository = repository;
        }
        public async Task Handle(TeamClickedAction notification, CancellationToken cancellationToken)
        {
            if (!IsTournamentNameEmpty(notification.Team))
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
        }

        private bool IsTournamentNameEmpty(string teamName)
            => string.IsNullOrEmpty(teamName);

        private bool IsTeamExist(string teamName)
            => TournamentTeamNames.Teams.Any(t => teamName == t);

    }
}
