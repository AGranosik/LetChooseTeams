using LCT.Core.Shared;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;

namespace LCT.Domain.Aggregates.TournamentAggregate.Services
{
    public interface ITournamentDomainService: IDomainService
    {
        Task ValidateAsync(Tournament tournament);
        List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams);
    }
}
