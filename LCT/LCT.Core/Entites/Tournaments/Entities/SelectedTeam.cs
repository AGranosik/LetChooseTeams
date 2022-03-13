using LCT.Core.Entites.Tournaments.ValueObjects;

namespace LCT.Core.Entites.Tournaments.Entities
{
    public class SelectedTeam : Entity
    {
        private SelectedTeam() { }
        private SelectedTeam(Guid tournamentId, Guid playerId, string teamName)
        {
            TournamentId = tournamentId;
            PlayerId = playerId;
            TeamName = new TeamName(teamName);
        }

        public static SelectedTeam Create(Guid tournamentId, Guid playerId, string teamName) 
            => new SelectedTeam(tournamentId, playerId, teamName);

        public Guid TournamentId { get; set; }
        public Tournament Tournament { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public TeamName TeamName { get; set; }
    }
}
