using FluentAssertions;
using LCT.Application.Common;
using LCT.Application.Tournaments.Queries;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Entites.Tournaments.ValueObjects;
using LCT.Core.Shared.Exceptions;
using LCT.Infrastructure.EF;
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
    public class GetTournamentTests : Testing<LctDbContext>
    {
        public GetTournamentTests() : base()
        {
            AddTablesToTruncate(new List<string>
            {
                nameof(LctDbContext.Tournaments),
                nameof(LctDbContext.Players)
            });

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
            var tournaments = await CreateTournament(10000);
            var tournament = await GetTournamentQueryHandler(new GetTournamentQuery
            {
                TournamentId = tournaments[tournaments.Count - 1].Id
            });

            tournament.Should().NotBeNull();
            tournament.TournamentName.Should().Be("name9999");
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
                TournamentId = tournaments[tournaments.Count - 1].Id
            }, mockedQRCodeCreator.Object);

            await func.Should().ThrowAsync<ArgumentNullException>();
        }


        private async Task<List<Tournament>> CreateTournament(int number)
        {
            var list = new List<Tournament>();
            for(int i =0; i < number; i++)
            {
                list.Add(Tournament.Create(new Name("name" + i), new TournamentLimit(2)));
            }

            var dbContext = GetDbContext();
            await dbContext.Tournaments.AddRangeAsync(list);
            await dbContext.SaveChangesAsync();
            return list;
        }

        private async Task<TournamentDto> GetTournamentQueryHandler(GetTournamentQuery request, IQRCodeCreator qrCodeCreator = null)
        {
            qrCodeCreator ??= _scope.ServiceProvider.GetRequiredService<IQRCodeCreator>();
            return await new GetTournamentQueryHandler(GetDbContext(), qrCodeCreator).Handle(request, new CancellationToken());
        }
    }
}
