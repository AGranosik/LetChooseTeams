namespace LCT.Domain.Aggregates.TournamentAggregate.Exceptions
{
    public class TournamentNameNotUniqueException: Exception
    {
        public TournamentNameNotUniqueException(): base("Tournament name not unique.")
        {

        }
    }
}
