using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Core.Entites.Tournaments.Events
{
    [BsonIgnoreExtraElements]
    [BsonDiscriminator("Test")]
    public class Test : BaseEvent
    {
        public Test(Guid id) : base(id)
        {
        }

        public DateTime Hehe { get; set; } = DateTime.Now;
    }
}
