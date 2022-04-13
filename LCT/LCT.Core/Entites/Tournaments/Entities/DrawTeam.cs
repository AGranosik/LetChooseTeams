using LCT.Core.Entites.Tournaments.ValueObjects;

namespace LCT.Core.Entites.Tournaments.Entities
{
    public class DrawTeam : Entity
    {
        private DrawTeam() { }
        private DrawTeam(Player player, string teamName)
        {
            Player = player;
            TeamName = new TeamName(teamName);
        }

        public static DrawTeam Create(Player player, string teamName)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));
            return new DrawTeam(player, teamName);
        }

        public Tournament Tournament { get; private set; }
        public Player Player { get; private set; }
        public TeamName TeamName { get; private set; }
    }
}
