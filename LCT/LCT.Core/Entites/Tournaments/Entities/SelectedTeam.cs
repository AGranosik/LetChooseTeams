namespace LCT.Core.Entites.Tournaments.Entities
{
    public class SelectedTeam : Entity
    {
        public Guid TournamentId { get; set; }
        public Tournament Tournament { get; set; }
        public Guid PlayerId { get; set; }
        public Player Player { get; set; }
        public string TeamName { get; set; }
    }
}
