using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Core.Entites.Tournaments.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("TournamentCreated")]
    public class TournamentCreated : BaseEvent
    {
        public string Name { get; set; }
        public int Limit { get; set; }
        public TournamentCreated(string tournamentName, int limit, Guid streamId): base(streamId)
        {
            Name = tournamentName;
            Limit = limit;
        }
    }
}
