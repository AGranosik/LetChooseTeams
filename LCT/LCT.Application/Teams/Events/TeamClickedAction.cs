using LCT.Application.Tournaments.Actions.TeamChooseAction;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Infrastructure.Repositories.Actions;
using MediatR;
using Serilog;

namespace LCT.Application.Teams.Events
{
    public class TeamSelectedActionHandler : INotificationHandler<TeamSelectedAction>
    {
        private readonly ILctActionRepository<TeamSelectedAction> _repository;
        public TeamSelectedActionHandler(ILctActionRepository<TeamSelectedAction> repository)
        {
            _repository = repository;
        }
        public async Task Handle(TeamSelectedAction notification, CancellationToken cancellationToken)
        {
            if (IsTournamentNameEmpty(notification.Team))
            {
                Log.Error("Team name cannot be empty in TeamSelectedAction");
                return;
            }

            if(!IsTeamExist(notification.Team))
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
