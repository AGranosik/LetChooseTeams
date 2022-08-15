namespace LCT.Core.Aggregates.TournamentAggregate.Exceptions
{
    public class TournamentNameNotUniqueException: Exception
    {
        public TournamentNameNotUniqueException(): base("Tournament name not unique.")
        {

        }
    }
}
