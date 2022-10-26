﻿using LCT.Domain.Common.BaseTypes;

namespace LCT.Domain.Aggregates.TournamentAggregate.Events
{
    public class SetTournamentNameEvent : DomainEvent, IVersionable
    {
        public string Name { get; set; }
        public SetTournamentNameEvent(string name, Guid streamId) : base(streamId) // shouldnt be of type TOurnamentName??
        {
            Name = name;
        }
    }
}
