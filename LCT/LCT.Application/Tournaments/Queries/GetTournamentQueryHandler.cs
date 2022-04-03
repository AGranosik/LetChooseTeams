using LCT.Application.Common;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Shared.Exceptions;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using QRCoder;
using System.Drawing;

namespace LCT.Application.Tournaments.Queries
{
    public class TournamentDto
    {
        public Guid Id { get; set; }
        public string TournamentName { get; set; }
        public List<PlayerDto> Players { get; set; }
        public string QRCode { get; set; }
    }

    public class PlayerDto
    {
        public string Name { get; set; }
    }
    public class GetTournamentQuery : IRequest<TournamentDto>
    {
        public Guid TournamentId { get; set; }
    }
    public class GetTournamentQueryHandler : IRequestHandler<GetTournamentQuery, TournamentDto>
    {
        private readonly LctDbContext _dbContext;
        private readonly IQRCodeCreator _qrCodeCreator;
        private readonly IConfiguration _configuration;
        public GetTournamentQueryHandler(LctDbContext dbContext, IQRCodeCreator qRCodeCreator, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _qrCodeCreator = qRCodeCreator;
            _configuration = configuration;
        }
        public async Task<TournamentDto> Handle(GetTournamentQuery request, CancellationToken cancellationToken)
        {
            var tournament = await _dbContext.Tournaments
                .Where(t => t.Id == request.TournamentId)
                .Select(t => new TournamentDto
                    {
                        Id = t.Id,
                        TournamentName = t.TournamentName.ToString(),
                        Players = t.Players.Select(p => new PlayerDto
                        {
                            Name = p.Name
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tournament == null)
                throw new EntityDoesNotExist(nameof(Tournament));
            var feLink = _configuration.GetSection("fe").Value + "player/register/" + request.TournamentId;
            tournament.QRCode = _qrCodeCreator.Generate(feLink);

            return tournament;
        }
    }
}
