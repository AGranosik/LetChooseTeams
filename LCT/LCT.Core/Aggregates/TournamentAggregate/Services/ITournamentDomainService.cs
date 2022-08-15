using LCT.Core.Aggregates.TournamentAggregate.Entities;
using LCT.Core.Shared;

namespace LCT.Core.Aggregates.TournamentAggregate.Services
{
    public interface ITournamentDomainService: IDomainService
    {
        Task ValidateAsync(Tournament tournament);
        List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams);
    }
}
