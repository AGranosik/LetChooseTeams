using System.Linq;
using FluentAssertions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using NUnit.Framework;

namespace LCT.IntegrationTests.Tournaments.DomainTests
{
    public class TournamentDomainTests
    {
        [Test]
        public void Tournament_CanAddPlayer_Success()
        {
            var tournament = Tournament.Create("test", 2);
            tournament.AddPlayer("test", "hehe");
        }

        [Test]
        public void Tournament_CannotAddedSamePlayer_ThrowsException()
        {
            var tournament = Tournament.Create("test", 2);
            var player = Player.Create("test", "hehe");
            tournament.AddPlayer(player.Name, player.Surname);
            var func = () => tournament.AddPlayer(player.Name, player.Surname);
            func.Should().Throw<PlayerAlreadyAssignedToTournamentException>();
        }

        [Test]
        public void Tournament_CannotExceedTournamentLimit_ThrowsException()
        {
            var tournament = Tournament.Create("test", 2);
            tournament.AddPlayer("test", "hehe");
            tournament.AddPlayer("t2est", "heh2e");
            var func = () => tournament.AddPlayer("t2e3st", "he3h2e");
            func.Should().Throw<TournamentLimitCannotBeExceededException>();

        }

        [Test]
        public void Tournament_CannotAssignWhilePlayerNotInTournament_ThrowsException()
        {
            var tournament = Tournament.Create("test", 2);
            var player1 = Player.Create("test", "hehe");
            tournament.AddPlayer(player1.Name, player1.Surname);

            var player2 = Player.Create("t2est", "heh2e");
            tournament.AddPlayer(player2.Name, player2.Surname);

            var notExistingPLayer = Player.Create("hehe", "fiu");

            var func = () => tournament.SelectTeam(notExistingPLayer.Name, notExistingPLayer.Surname, TournamentTeamNames.Teams.First());
            func.Should().Throw<PlayerNotInTournamentException>();
        }

        [Test]
        public void Tournament_SamePlayerCannotSelectTeamTwice_ThrowsException()
        {
            var tournament = Tournament.Create("test", 2);
            var player = Player.Create("test", "hehe");
            tournament.AddPlayer(player.Name, player.Surname);
            tournament.AddPlayer("t2est", "heh2e");

            tournament.SelectTeam(player.Name, player.Surname, TournamentTeamNames.Teams.Last());
            var func = () => tournament.SelectTeam(player.Name, player.Surname, TournamentTeamNames.Teams.First());
            func.Should().Throw<PlayerSelectedTeamBeforeException>();
        }

        [Test]
        public void Tournament_SelectTeam_Success()
        {
            var tournament = Tournament.Create("test", 2);
            var player = Player.Create("test", "hehe");
            tournament.AddPlayer(player.Name, player.Surname);

            tournament.SelectTeam(player.Name, player.Surname, TournamentTeamNames.Teams.First());

            tournament.SelectedTeams.Should().HaveCount(1);
        }

        [Test]
        public void Tournament_CannotSelectSameTeamTwice()
        {
            var tournament = Tournament.Create("test", 2);
            var player = Player.Create("test", "hehe");
            tournament.AddPlayer(player.Name, player.Surname);
            var player2 = Player.Create("t2est", "heh2e");
            tournament.AddPlayer(player2.Name, player2.Surname);

            tournament.SelectTeam(player.Name, player.Surname, TournamentTeamNames.Teams.First());
            var func = () => tournament.SelectTeam(player2.Name, player2.Surname, TournamentTeamNames.Teams.First());
            func.Should().Throw<TeamAlreadySelectedException>();
        }


        //[Test]
        //public void TournamentDomainServiceCannotBeNull()
        //{
        //    //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
        //    //var func = () => tournament.DrawnTeamForPLayers(null);
        //    //func.Should().Throw<ArgumentNullException>();
        //}

        //[Test]
        //public void TournamentNotAllPlayersRegistered_ThrowEsception()
        //{
        //    //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
        //    //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());

        //    //func.Should().Throw<NotAllPlayersRegisteredException>();
        //}

        //[Test]
        //public void TournamentNotAllPlayersRegistered2_ThrowEsception()
        //{
        //    //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
        //    //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());
        //    //var player = Player.Register(new Name("test"), new Name("hehe"));
        //    //player.Id = Guid.NewGuid();
        //    //tournament.AddPlayer(player);
        //    //func.Should().Throw<NotAllPlayersRegisteredException>();
        //}

        //[Test]
        //public void TournamentCannotDrawTeamsWhenAnySelected()
        //{
        //    //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
        //    //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());
        //    //var player = Player.Register(new Name("test"), new Name("hehe"));
        //    //player.Id = Guid.NewGuid();
        //    //tournament.AddPlayer(player);
        //    //var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
        //    //player2.Id = Guid.NewGuid();
        //    //tournament.AddPlayer(player2);

        //    //func.Should().Throw<NotAllPlayersSelectedTeamException>();
        //}

        //[Test]
        //public void TournamentCannotDrawTeamsWhenNotAllPlayersSelected_ThrowsException()
        //{
        //    //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
        //    //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());
        //    //var player = Player.Register(new Name("test"), new Name("hehe"));
        //    //player.Id = Guid.NewGuid();
        //    //tournament.AddPlayer(player);
        //    //var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
        //    //player2.Id = Guid.NewGuid();
        //    //tournament.AddPlayer(player2);
        //    //tournament.SelectTeam(player2.Id, TournamentTeamNames.Teams.First());
        //    //func.Should().Throw<NotAllPlayersSelectedTeamException>();
        //}

        //[Test]
        //[Repeat(20)]
        //public void TournamentCannotDrawTeamsWhenAllPlayersSelected_Success()
        //{
        //    //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
        //    //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());
        //    //var player = Player.Register(new Name("test"), new Name("hehe"));
        //    //player.Id = Guid.NewGuid();
        //    //tournament.AddPlayer(player);
        //    //var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
        //    //tournament.AddPlayer(player2);
        //    //player2.Id = Guid.NewGuid();

        //    //tournament.SelectTeam(player2.Id, TournamentTeamNames.Teams.First());
        //    //tournament.SelectTeam(player.Id, TournamentTeamNames.Teams.Last());


        //    //tournament.DrawnTeamForPLayers(new TournamentDomainService());

        //    //tournament.DrawTeams.Count.Should().Be(2);
        //    //var drawnTeam = tournament.DrawTeams.First(dt => dt.Player == player);
        //    //drawnTeam.TeamName.Value.Should().Be(TournamentTeamNames.Teams.First());

        //}

        //[Test]
        //[Repeat(20)]
        //public void TournamentCannotDrawTeamsWhenAllPlayersSelected_Success2()
        //{
        //    //var count = TournamentTeamNames.Teams.Count;
        //    //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(count));

        //    //foreach(var team in TournamentTeamNames.Teams)
        //    //{
        //    //    var player = Player.Register(new Name("test" + team), new Name("hehe"));
        //    //    player.Id = Guid.NewGuid();
        //    //    tournament.AddPlayer(player);
        //    //    tournament.SelectTeam(player.Id, team);
        //    //}

        //    //tournament.DrawnTeamForPLayers(new TournamentDomainService());

        //    //tournament.DrawTeams.Count.Should().Be(count);

        //    //foreach(var selectedTeam in tournament.SelectedTeams)
        //    //{
        //    //    var drawTeam = tournament.DrawTeams.First(dt => dt.Player == selectedTeam.Player);
        //    //    drawTeam.TeamName.Value.Should().NotBe(selectedTeam.TeamName.Value);
        //    //}

        //}

    }
}
