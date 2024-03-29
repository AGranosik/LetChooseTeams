﻿using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Common.Aggregates.TournamentAggregate.ValueObjects;

namespace LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Teams
{
    public class TeamName : Name
    {
        TeamName(): base() { }
        private TeamName(string name) : base(name)
        {
        }

        public static TeamName Create(string name)
            => new TeamName(name);

        protected override void Validate(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Nazwa druzyny nie moze byc pusta.");

            if (!TournamentTeamNames.TeamExists(name))
                throw new ArgumentNullException("Niepoprawna nazwa druzyny.");
        }
    }
}
