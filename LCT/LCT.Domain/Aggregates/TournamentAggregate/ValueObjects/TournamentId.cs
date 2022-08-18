using LCT.Core.Shared.BaseTypes;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects
{
    public class TournamentId : BaseId<TournamentId, Guid>
    {
        private TournamentId(Guid id) : base(id)
        {
        }

        public static TournamentId Create()
            => new TournamentId(Guid.NewGuid());
    }
}
