using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Common.BaseTypes;
using LCT.Domain.Common.Exceptions;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects
{
    public class TournamentLimit : ValueType<TournamentLimit>
    {
        public TournamentLimit(int limit)
        {
            Validate(limit);
            Limit = limit;
        }
        public int Limit { get; init; }

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

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;

            var limit = obj as TournamentLimit;

            if (limit is null)
                return false;

            return limit.Limit == Limit;
        }

        public static implicit operator TournamentLimit(int limit) => new(limit);
    }
}
