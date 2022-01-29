using FluentAssertions;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.Exceptions;
using LCT.Core.Entites.Tournaments.ValueObjects;
using NUnit.Framework;

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
    }
}
