namespace LCT.Domain.Aggregates.TournamentAggregate.Exceptions
{
    public class PlayerAlreadyAssignedToTournamentException : Exception
    {
        public PlayerAlreadyAssignedToTournamentException() : base() // new type of exception // data not unique exception ors sth like thath
        {

        }
    }
}
