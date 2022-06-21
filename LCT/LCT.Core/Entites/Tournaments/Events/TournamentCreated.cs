using LCT.Core.Entites.Tournaments.ValueObjects;

namespace LCT.Core.Entites.Tournaments.Events
{
    public record TournamentCreated(Name TournamentName, TournamentLimit Limit);
}
