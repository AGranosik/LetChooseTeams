using LCT.Core.Shared.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;

namespace LCT.Domain.Aggregates.TournamentAggregate.Services
{
    public interface ITournamentDomainService: IDomainService
    {
        Task TournamentUniqnessValidationAsync(Tournament tournament);
        List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams);
    }
}
