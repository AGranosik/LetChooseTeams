﻿using MongoDB.Bson.Serialization.Attributes;

namespace LCT.Core.Entites.Tournaments.Events
{
    [BsonIgnoreExtraElements]
    public abstract class BaseEvent
    {
        public BaseEvent(Guid streamId, Guid id)
        {
            StreamId = streamId;
            Id = id;
        }
        public Guid Id { get; set; }
        public Guid StreamId { get; private set; }
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}
