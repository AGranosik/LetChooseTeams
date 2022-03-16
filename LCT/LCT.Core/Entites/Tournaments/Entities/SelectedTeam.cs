using LCT.Core.Entites.Tournaments.ValueObjects;

namespace LCT.Core.Entites.Tournaments.Entities
{
    public class SelectedTeam : Entity
    {
        private SelectedTeam() { }
        private SelectedTeam(Guid playerId, string teamName)
        {
            PlayerId = playerId;
            TeamName = new TeamName(teamName);
        }

        public static SelectedTeam Create(Guid playerId, string teamName) 
            => new SelectedTeam(playerId, teamName);

        public Guid TournamentId { get; private set; }
        public Tournament Tournament { get; private set; }
        public Guid PlayerId { get; private set; }
        public Player Player { get; private set; }
        public TeamName TeamName { get; private set; }

        public static bool operator ==(SelectedTeam a, SelectedTeam b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is not null && b is not null)
            {
                return a.TournamentId == b.TournamentId
                    && b.Player == a.Player
                    && a.TeamName == b.TeamName;
            }

            return false;
        }

        public static bool operator !=(SelectedTeam a, SelectedTeam b) => !(a == b);
    }
}
