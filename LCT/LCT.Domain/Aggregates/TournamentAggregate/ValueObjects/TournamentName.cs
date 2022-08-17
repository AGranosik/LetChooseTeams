using LCT.Core.Shared;
using Microsoft.EntityFrameworkCore;
using static LCT.Core.Shared.Validation.FieldValidationExtension;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects
{
    public class TournamentName : Name
    {
        public TournamentName(string name) : base(name)
        {
        }

        public static implicit operator string(TournamentName name) => name.Value;

        public static implicit operator TournamentName(string name) => new(name);
    }


}
