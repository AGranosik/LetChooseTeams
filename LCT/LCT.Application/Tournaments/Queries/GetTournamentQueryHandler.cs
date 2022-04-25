using LCT.Application.Common;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Core.Shared.Exceptions;
using LCT.Infrastructure.EF;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LCT.Application.Tournaments.Queries
{
    public class TournamentDto
    {
        public Guid Id { get; set; }
        public string TournamentName { get; set; }
        public List<PlayerDto> Players { get; set; }
        public string QRCode { get; set; }
        public int PlayerLimit { get; set; }
    }

    public class PlayerDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string SelectedTeam { get; set; }
        public string DrawnTeam { get; set; }
    }
    public class GetTournamentQuery : IRequest<TournamentDto>
    {
        public Guid TournamentId { get; set; }
    }
    public class GetTournamentQueryHandler : IRequestHandler<GetTournamentQuery, TournamentDto>
    {
        private readonly LctDbContext _dbContext;
        private readonly IQRCodeCreator _qrCodeCreator;
        public GetTournamentQueryHandler(LctDbContext dbContext, IQRCodeCreator qRCodeCreator)
        {
            _dbContext = dbContext;
            _qrCodeCreator = qRCodeCreator;
        }
        public async Task<TournamentDto> Handle(GetTournamentQuery request, CancellationToken cancellationToken)
        {
            var tournament = await _dbContext.Tournaments
                .Where(t => t.Id == request.TournamentId)
                .Select(t => new TournamentDto
                    {
                        Id = t.Id,
                        TournamentName = t.TournamentName.ToString(),
                        PlayerLimit = t.Limit.Limit,
                        Players = t.Players.Select(p => new PlayerDto
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Surname = p.Surname,
                            SelectedTeam = t.SelectedTeams.Where(st => st.Player.Id == p.Id).Select(st => st.TeamName).FirstOrDefault(),
                            DrawnTeam = t.DrawTeams.Where(dt => dt.Player.Id == p.Id).Select(dt => dt.TeamName).FirstOrDefault()
                        }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (tournament == null)
                throw new EntityDoesNotExist(nameof(Tournament));
            
            var feLink = "http://" + IpAdressProvider.GetHostAdress() + ":3000/player/register/" + request.TournamentId;
            tournament.QRCode = _qrCodeCreator.Generate(feLink);

            return tournament;
        }
    }
}
