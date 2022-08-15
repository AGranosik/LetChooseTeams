using NUnit.Framework;

namespace LCT.IntegrationTests.Tournaments.DomainTests
{
    public class TournamentDomainTests
    {
        [Test]
        public void Tournament_CanAddPlayer()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //tournament.AddPlayer(Player.Register(new Name("test"), new Name("hehe")));
        }

        [Test]
        public void Tournament_CannotAddedSamePlayer()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //tournament.AddPlayer(player);
            //var func = () => tournament.AddPlayer(player);
            //func.Should().Throw<PlayerAlreadyAssignedToTournamentException>();
        }

        [Test]
        public void Tournament_CannotExceedTournamentLimit()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //tournament.AddPlayer(Player.Register(new Name("test"), new Name("hehe")));
            //tournament.AddPlayer(Player.Register(new Name("t2est"), new Name("heh2e")));
            //var func = () => tournament.AddPlayer(Player.Register(new Name("t2e3st"), new Name("he3h2e")));
            //func.Should().Throw<TournamentLimitCannotBeExceededException>();
            
        }

        [Test]
        public void Tournament_CannotAssignWhilePlayerNotInTournament()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var player1 = Player.Register(new Name("test"), new Name("hehe"));
            //player1.Id = Guid.NewGuid();
            //tournament.AddPlayer(player1);

            //var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
            //player2.Id = Guid.NewGuid();
            //tournament.AddPlayer(player2);

            //var notExistingPLayer = Player.Register(new Name("hehe"), new Name("fiu"));

            //var func = () => tournament.SelectTeam(notExistingPLayer.Id, TournamentTeamNames.Teams.First());
            //func.Should().Throw<PlayerNotInTournamentException>();
        }

        [Test]
        public void Tournament_SamePlayerCannotSelectTeamTwice()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //tournament.AddPlayer(player);
            //tournament.AddPlayer(Player.Register(new Name("t2est"), new Name("heh2e")));

            //tournament.SelectTeam(player.Id, TournamentTeamNames.Teams.Last());
            //var func = () => tournament.SelectTeam(player.Id, TournamentTeamNames.Teams.First());
            //func.Should().Throw<PlayerSelectedTeamBeforeException>();
        }

        [Test]
        public void Tournament_SelectTeam_Success()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //tournament.AddPlayer(player);

            //tournament.SelectTeam(player.Id, TournamentTeamNames.Teams.First());

            //tournament.SelectedTeams.Should().HaveCount(1);
        }

        [Test]
        public void Tournament_CannotSelectSameTeamTwice()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //player.Id = Guid.NewGuid();
            //tournament.AddPlayer(player);
            //var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
            //player2.Id = Guid.NewGuid();
            //tournament.AddPlayer(player2);

            //tournament.SelectTeam(player.Id, TournamentTeamNames.Teams.First());
            //var func = () => tournament.SelectTeam(player2.Id, TournamentTeamNames.Teams.First());
            //func.Should().Throw<TeamAlreadySelectedException>();
        }


        [Test]
        public void TournamentDomainServiceCannotBeNull()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var func = () => tournament.DrawnTeamForPLayers(null);
            //func.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void TournamentNotAllPlayersRegistered_ThrowEsception()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());

            //func.Should().Throw<NotAllPlayersRegisteredException>();
        }

        [Test]
        public void TournamentNotAllPlayersRegistered2_ThrowEsception()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //player.Id = Guid.NewGuid();
            //tournament.AddPlayer(player);
            //func.Should().Throw<NotAllPlayersRegisteredException>();
        }

        [Test]
        public void TournamentCannotDrawTeamsWhenAnySelected()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //player.Id = Guid.NewGuid();
            //tournament.AddPlayer(player);
            //var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
            //player2.Id = Guid.NewGuid();
            //tournament.AddPlayer(player2);

            //func.Should().Throw<NotAllPlayersSelectedTeamException>();
        }

        [Test]
        public void TournamentCannotDrawTeamsWhenNotAllPlayersSelected_ThrowsException()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //player.Id = Guid.NewGuid();
            //tournament.AddPlayer(player);
            //var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
            //player2.Id = Guid.NewGuid();
            //tournament.AddPlayer(player2);
            //tournament.SelectTeam(player2.Id, TournamentTeamNames.Teams.First());
            //func.Should().Throw<NotAllPlayersSelectedTeamException>();
        }

        [Test]
        [Repeat(20)]
        public void TournamentCannotDrawTeamsWhenAllPlayersSelected_Success()
        {
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(2));
            //var func = () => tournament.DrawnTeamForPLayers(new TournamentDomainService());
            //var player = Player.Register(new Name("test"), new Name("hehe"));
            //player.Id = Guid.NewGuid();
            //tournament.AddPlayer(player);
            //var player2 = Player.Register(new Name("t2est"), new Name("heh2e"));
            //tournament.AddPlayer(player2);
            //player2.Id = Guid.NewGuid();

            //tournament.SelectTeam(player2.Id, TournamentTeamNames.Teams.First());
            //tournament.SelectTeam(player.Id, TournamentTeamNames.Teams.Last());


            //tournament.DrawnTeamForPLayers(new TournamentDomainService());

            //tournament.DrawTeams.Count.Should().Be(2);
            //var drawnTeam = tournament.DrawTeams.First(dt => dt.Player == player);
            //drawnTeam.TeamName.Value.Should().Be(TournamentTeamNames.Teams.First());

        }

        [Test]
        [Repeat(20)]
        public void TournamentCannotDrawTeamsWhenAllPlayersSelected_Success2()
        {
            //var count = TournamentTeamNames.Teams.Count;
            //var tournament = Tournament.Create(new Name("test"), new TournamentLimit(count));

            //foreach(var team in TournamentTeamNames.Teams)
            //{
            //    var player = Player.Register(new Name("test" + team), new Name("hehe"));
            //    player.Id = Guid.NewGuid();
            //    tournament.AddPlayer(player);
            //    tournament.SelectTeam(player.Id, team);
            //}

            //tournament.DrawnTeamForPLayers(new TournamentDomainService());

            //tournament.DrawTeams.Count.Should().Be(count);

            //foreach(var selectedTeam in tournament.SelectedTeams)
            //{
            //    var drawTeam = tournament.DrawTeams.First(dt => dt.Player == selectedTeam.Player);
            //    drawTeam.TeamName.Value.Should().NotBe(selectedTeam.TeamName.Value);
            //}

        }

    }
}
