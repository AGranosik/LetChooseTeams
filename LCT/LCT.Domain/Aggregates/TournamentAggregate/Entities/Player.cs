﻿using LCT.Core.Shared.BaseTypes;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;

namespace LCT.Domain.Aggregates.TournamentAggregate.Entities
{
    public class Player : Entity<Guid>
    {
        private Player(TournamentName name, TournamentName surname, Guid id): base(id){
            Name = name;
            Surname = surname;
        }
        public TournamentName Name { get; private set; }
        public TournamentName Surname { get; private set; }

        public static Player Register(TournamentName name, TournamentName surname, Guid id)
            => new (name, surname, id);

        public static bool operator ==(Player a, Player b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a is not null && b is not null)
            {
                return a.Name == b.Name
                    && b.Surname == a.Surname;
            }

            return false;
        }

        public static bool operator !=(Player a, Player b) => !(a == b);
    }
}
