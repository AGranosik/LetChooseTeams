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
            var player1 = Player.Register(new Name("test"), new Name("hehe"));
            player1.Id = Guid.NewGuid();
            tournament.AddPlayer(player1);

            var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
            player2.Id = Guid.NewGuid();
            tournament.AddPlayer(player2);

            var notExistingPLayer = Player.Register(new Name("hehe"), new Name("fiu"));

            var func = () => tournament.SelectTeam(notExistingPLayer.Id, TournamentTeamNames._teams.First());
            func.Should().Throw<PlayerNotInTournamentException>();
        }

        [Test]
        public void Tournament_SamePlayerCannotSelectTeamTwice()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            var player = Player.Register(new Name("test"), new Name("hehe"));
            tournament.AddPlayer(player);
            tournament.AddPlayer(Player.Register(new Name("t2est"), new Name("heh2e")));

            tournament.SelectTeam(player.Id, TournamentTeamNames._teams.Last());
            var func = () => tournament.SelectTeam(player.Id, TournamentTeamNames._teams.First());
            func.Should().Throw<PlayerSelectedTeamBeforeException>();
        }

        [Test]
        public void Tournament_SelectTeam_Success()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            var player = Player.Register(new Name("test"), new Name("hehe"));
            tournament.AddPlayer(player);

            tournament.SelectTeam(player.Id, TournamentTeamNames._teams.First());

            tournament.SelectedTeams.Should().HaveCount(1);
        }

        [Test]
        public void Tournament_CannotSelectSameTeamTwice()
        {
            var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            var player = Player.Register(new Name("test"), new Name("hehe"));
            player.Id = Guid.NewGuid();
            tournament.AddPlayer(player);
            var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
            player2.Id = Guid.NewGuid();
            tournament.AddPlayer(player2);

            tournament.SelectTeam(player.Id, TournamentTeamNames._teams.First());
            var func = () => tournament.SelectTeam(player2.Id, TournamentTeamNames._teams.First());
            func.Should().Throw<TeamAlreadySelectedException>();
        }

    }
}
