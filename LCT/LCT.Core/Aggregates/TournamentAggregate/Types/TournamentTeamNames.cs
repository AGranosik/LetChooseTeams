namespace LCT.Core.Aggregates.TournamentAggregate.Types
{
    public static class TournamentTeamNames
    {
        public static readonly List<string> Teams = new List<string>() {
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
            => Teams.Any(t => t == team);
    }
}
