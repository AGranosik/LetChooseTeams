using FluentAssertions;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Exceptions;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Entities.Tournaments.Types;
using NUnit.Framework;
using System;
using System.Linq;

namespace LCT.IntegrationTests.Tournaments.DomainTests
{
    public class TournamentDomainTests
    {
        [Test]
        public void Tournament_CanAddPlayer()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            tournament.AddPlayer(Player.Register(new Name("test"), new Name("hehe")));
        }

        [Test]
        public void Tournament_CannotAddedSamePlayer()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            var player = Player.Register(new Name("test"), new Name("hehe"));
            tournament.AddPlayer(player);
            var func = () => tournament.AddPlayer(player);
            func.Should().Throw<PlayerAlreadyAssignedToTournamentException>();
        }

        [Test]
        public void Tournament_CannotExceedTournamentLimit()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            tournament.AddPlayer(Player.Register(new Name("test"), new Name("hehe")));
            tournament.AddPlayer(Player.Register(new Name("t2est"), new Name("heh2e")));
            var func = () => tournament.AddPlayer(Player.Register(new Name("t2e3st"), new Name("he3h2e")));
            func.Should().Throw<TournamentLimitCannotBeExceededException>();
            
        }

        [Test]
        public void Tournament_CannotAssignWhilePlayerNotInTournament()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            tournament.AddPlayer(Player.Register(new Name("test"), new Name("hehe")));
            tournament.AddPlayer(Player.Register(new Name("t2est"), new Name("heh2e")));

            var func = () => tournament.SelectTeam(SelectedTeam.Create(Guid.Empty, TournamentTeamNames._teams.First()));
            func.Should().Throw<PlayerNotInTournamentException>();
        }

        [Test]
        public void Tournament_SamePlayerCannotSelectTeamTwice()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            var player = Player.Register(new Name("test"), new Name("hehe"));
            tournament.AddPlayer(player);
            tournament.AddPlayer(Player.Register(new Name("t2est"), new Name("heh2e")));

            tournament.SelectTeam(SelectedTeam.Create(player.Id, "test"));
            var func = () => tournament.SelectTeam(SelectedTeam.Create(player.Id, "test"));
            func.Should().Throw<PlayerSelectedTeamBeforeException>();
        }
    }
}
