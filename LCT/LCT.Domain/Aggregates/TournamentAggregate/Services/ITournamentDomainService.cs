using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using LCT.Domain.Common.Interfaces;

namespace LCT.Domain.Aggregates.TournamentAggregate.Services
{
    public interface ITournamentDomainService: IDomainService
    {
        Task TournamentNameUniqnessValidationAsync(Tournament tournament);
        Task PlayerTeamSelectionValidationAsync(string team, Guid tournamentId);
        List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams);
    }
}
