namespace LCT.Core.Entites.Tournaments.Exceptions
{
    public class TournamentNameNotUniqueException: Exception
    {
        public TournamentNameNotUniqueException(): base("Tournament name not unique.")
        {

        }
    }
}
