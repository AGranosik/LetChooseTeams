using LCT.Core.Entites.Tournaments.Entities;

namespace LCT.Core.Entites.Tournaments.Services
{
    public interface ITournamentDomainService: IDomainService
    {
        Task ValidateAsync(Tournament tournament);
        List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams);
    }
}
