using LCT.Core.Shared.Interfaces;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;

namespace LCT.Domain.Aggregates.TournamentAggregate.Services
{
    public interface ITournamentDomainService: IDomainService
    {
        Task TournamentNameUniqnessValidationAsync(Tournament tournament);
        Task PlayerTeamSelectionValidationAsync(string team, Guid tournamentId);
        List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams);
    }
}
