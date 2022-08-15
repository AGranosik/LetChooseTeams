﻿using LCT.Core.Aggregates.TournamentAggregate.Events;

namespace LCT.Core.Shared
{
    public abstract class Aggregate
    {
        private readonly IList<BaseEvent> _changes = new List<BaseEvent>();
        public long Version { get; private set; } = -1;
        protected abstract void When(object @event);
        public void Apply(BaseEvent @event)
        {
            When(@event);

            _changes.Add(@event);
        }

        public void Load(long version, IEnumerable<object> history)
        {
            Version = version;

            foreach (var item in history)
            {
                When(item);
            }
        }
        public BaseEvent[] GetChanges() => _changes.OrderBy(c => c.TimeStamp).ToArray();
    }
}