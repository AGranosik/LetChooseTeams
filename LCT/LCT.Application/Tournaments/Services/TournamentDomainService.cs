using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Extensions;

namespace LCT.Core.Entites.Tournaments.Services
{
    public class TournamentDomainService : ITournamentDomainService
    {
        public List<DrawnTeam> DrawTeamForPlayers(List<SelectedTeam> selectedTeams)
        {
            var playerList = selectedTeams
                .Select(st => st.Player)
                .Shuffle();

            var teams = selectedTeams
                .Select(st => st.TeamName)
                .Shuffle()
                .ToList();

            var result = playerList.Select((p, index) => DrawnTeam.Create(p, teams[index])).ToList();

            for(int i = 0; i < teams.Count; i++)
            {
                if (result[i].Player == selectedTeams[i].Player && result[i].TeamName == selectedTeams[i].TeamName)
                    return DrawTeamForPlayers(selectedTeams);
            }

            return result;
        }
    }
}
