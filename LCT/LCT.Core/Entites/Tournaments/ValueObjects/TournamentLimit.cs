using LCT.Core.Entites.Tournaments.Exceptions;
using LCT.Core.Shared.Exceptions;

namespace LCT.Core.Entites.Tournaments.ValueObjects
{
    public class TournamentLimit
    {
        public TournamentLimit(int limit)
        {
            Validate(limit);
            Limit = limit;
        }
        public int Limit { get; private set; }

        private void Validate(int limit)
        {
            if (limit <= 1)
                throw new ValueSmallerThanMinimalValueException();

        }

        public void ChceckIfPlayerCanBeAdded(int currentNumberOfPlayers)
        {
            if (currentNumberOfPlayers + 1 > Limit)
                throw new TournamentLimitCannotBeExceededException();
        }
    }
}
