using FluentAssertions;
using LCT.Application.Common;
using LCT.Application.Common.Configs;
using LCT.Application.Tournaments.Hubs;
using LCT.Application.Tournaments.Queries;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.Types;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects.Players;
using LCT.Domain.Common.Exceptions;
using LCT.IntegrationTests.Mocks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.DFM;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LCT.IntegrationTests.Tournaments.IntegrationTests
{
    [TestFixture]
    public class GetTournamentTests : Testing<Tournament>
    {
        public GetTournamentTests() : base()
        {
            SwapSingleton<IHubContext<TournamentHub>>(IHubContextMock.GetMockedHubContext<TournamentHub>());
            this.Environment("Development")
                .ProjectName("LCT.Api")
                .Build();
        }

        [Test]
        public async Task TournamentDoesNotExist_ThrowsAsync()
        {
            var action = () => GetTournamentQueryHandler(new GetTournamentQuery
            {
                TournamentId = Guid.Empty
            });

            await action.Should().ThrowAsync<EntityDoesNotExist>();
        }

        [Test]
        public async Task WrongTournamentId_ThrowsAsync()
        {
            await CreateTournament(1);

            var action = () => GetTournamentQueryHandler(new GetTournamentQuery
            {
                TournamentId = Guid.NewGuid()
            });

            await action.Should().ThrowAsync<EntityDoesNotExist>();
        }

        [Test]
        public async Task GetTournament_SuccessAsync()
        {
            var tournaments = await CreateTournament(9);
            var tournament = await GetTournamentQueryHandler(new GetTournamentQuery
            {
                TournamentId = tournaments[tournaments.Count - 1].Id.Value
            });

            tournament.Should().NotBeNull();
            tournament.TournamentName.Should().Be("name8");
            tournament.Players.Should().BeEmpty();
            tournament.QRCode.Should().NotBeNullOrEmpty();
        }

        [Test]
        public async Task GetTournament_CannotCreateQRCode_ThrowsException()
        {
            var mockedQRCodeCreator = new Mock<IQRCodeCreator>();
            mockedQRCodeCreator.Setup(c => c.Generate(It.IsAny<string>()))
                .Throws(new ArgumentNullException());

            var tournaments = await CreateTournament(2);
            var func = async () => await GetTournamentQueryHandler(new GetTournamentQuery
            {
                TournamentId = tournaments[tournaments.Count - 1].Id.Value
            }, mockedQRCodeCreator.Object);

            await func.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task GetTournament_NoDrawnTeam_NoException()
        {
            var tournaments = await CreateTournamentWitPlayers(9);
            var tournament = await GetTournamentQueryHandler(new GetTournamentQuery
            {
                TournamentId = tournaments[tournaments.Count - 1].Id.Value
            });

            tournament.Should().NotBeNull();
            tournament.TournamentName.Should().Be("name8");
            tournament.Players.Should().NotBeEmpty();
            tournament.Players.TrueForAll(p => p.DrawnTeam == null);
        }

        private async Task<List<Tournament>> CreateTournament(int number)
        {
            var list = new List<Tournament>();
            for (int i = 0; i < number; i++)
            {
                var tournament = Tournament.Create("name" + i, 2);
                list.Add(tournament);
                await AddAsync(tournament);
            }
            return list;
        }


        private async Task<List<Tournament>> CreateTournamentWitPlayers(int number)
        {
            var list = new List<Tournament>();
            for (int i = 0; i < number; i++)
            {
                var player = Player.Create(i.ToString(), i.ToString());
                var tournament = Tournament.Create("name" + i, 2);
                tournament.AddPlayer(player.Name, player.Surname);
                tournament.SelectTeam(player.Name, player.Surname, TournamentTeamNames.Teams[i]);
                list.Add(tournament);
                await AddAsync(tournament);
            }

            return list;
        }

        private async Task<TournamentDto> GetTournamentQueryHandler(GetTournamentQuery request, IQRCodeCreator qrCodeCreator = null)
        {
            qrCodeCreator ??= _scope.ServiceProvider.GetRequiredService<IQRCodeCreator>();
            return await new GetTournamentQueryHandler(qrCodeCreator, GetRepository(), new FrontendConfiguration()).Handle(request, new CancellationToken());
        }
    }
}
