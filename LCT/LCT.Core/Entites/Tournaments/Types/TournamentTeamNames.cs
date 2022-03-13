namespace LCT.Core.Entities.Tournaments.Types
{
    public static class TournamentTeamNames
    {
        public static readonly List<string> _teams = new List<string>() {
            "Bayern",
            "PSG",
            "Manchaster City",
            "Liverpool",
            "Machester United",
            "Real Madrid",
            "Chelsea",
            "Atletico Madrid",
            "Juventus",
            "Barcelona"
        };

        public static bool TeamExists(string team)
            => _teams.Any(t => t == team);
    }
}
