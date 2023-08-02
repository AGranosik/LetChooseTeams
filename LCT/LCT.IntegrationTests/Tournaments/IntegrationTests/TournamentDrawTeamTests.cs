using FluentAssertions;
using LCT.Application.Tournaments.Queries;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Common.Exceptions;
using LCT.Infrastructure.ClientCommunication.Hubs;
using LCT.IntegrationTests.Mocks;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class TournamentDrawTeamTests : Testing<Tournament>
    {
        public TournamentDrawTeamTests() : base()
        {
            SwapSingleton(IHubContextMock.GetMockedHubContext<TournamentHub>());
            AddTableToTruncate("TournamentStream");
            AddTableToTruncate("Tournament_SetTournamentNameEvent_index");
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task TournamentDoesNotExist_ThrowsException()
        {
            var action = () => DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = Guid.Empty });

            await action.Should().ThrowAsync<EntityDoesNotExist>();
        }

        [Test]
        public async Task NotAllPlayersPicked_ThrowsException()
        {
            var tournament = await CreateCompleteTournament(3, 3, 2);
            var action = () => DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = tournament.Id.Value });

            await action.Should().ThrowAsync<NotAllPlayersSelectedTeamException>();
        }

        [Test]
        public async Task NotAllPlayersRegistered_ThrowsException()
        {
            var tournament = await CreateCompleteTournament(3, 2, 2);
            var action = () => DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = tournament.Id.Value });

            await action.Should().ThrowAsync<NotAllPlayersRegisteredException>();
        }

        [Test]
        public async Task TeamsDrawnProperly_NoException()
        {
            var tournament = await CreateCompleteTournament(3, 3, 3);
            var action = () => DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = tournament.Id.Value });

            await action.Should().NotThrowAsync();
        }

        [Test]
        public async Task TeamsDrawnProperly_DataSavedInDb()
        {
            var tournament = await CreateCompleteTournament(7, 7, 7);
            await DrawTeamQueryHandlerResult(new DrawTeamForPlayersQuery { TournamentId = tournament.Id.Value });

            var tournamentDb = await GetTournamentByIdAsync(tournament.Id.Value);
            var drawTeams = tournamentDb.DrawTeams;
            drawTeams.Should().NotBeEmpty();
            drawTeams.Count.Should().Be(7);

        }

        private async Task<Tournament> GetTournamentByIdAsync(Guid tournamentId)
        {
            var repo = GetRepository();
            return await repo.LoadAsync(tournamentId);
        }

        private async Task<Tournament> CreateCompleteTournament(int limit, int players, int selectedTeams)
        {
            var tournament = Tournament.Create("test", limit);

            for (int i = 0; i < players; i++)
            {
                var player = Player.Create(i.ToString(), i.ToString());
                tournament.AddPlayer(player.Name, player.Surname);
            }

            for (int i = 0; i < selectedTeams; i++)
            {
                var player = tournament.Players.ElementAt(i);

                tournament.SelectTeam(player.Name, player.Surname, TournamentTeamNames.Teams[i]);
            }
            await SaveAsync(tournament, tournament.Version);
            return tournament;
        }

        private async Task<List<DrawnTeamDto>> DrawTeamQueryHandlerResult(DrawTeamForPlayersQuery query)
            => await new DrawTeamForPlayersQueryHandler(GetTournamentDomainService(), GetRepository()).Handle(query, CancellationToken.None);

    }
}
