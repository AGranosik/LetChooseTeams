using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams;
using LCT.Domain.Common.Interfaces;

namespace LCT.Domain.Aggregates.TournamentAggregate.Services
{
    public interface ITournamentDomainService: IDomainService
    {
        List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams);
    }
}
