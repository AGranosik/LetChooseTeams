using LCT.Application.Common;
using LCT.Application.Common.Configs;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Aggregates.TournamentAggregate.ValueObjects;
using LCT.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Configuration;

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
        private readonly IQRCodeCreator _qrCodeCreator;
        private readonly IRepository<Tournament> _repository;
        private readonly FrontendConfiguration _feCfg;
        public GetTournamentQueryHandler(IQRCodeCreator qRCodeCreator, IRepository<Tournament> repository, FrontendConfiguration feCfg)
        {
            _qrCodeCreator = qRCodeCreator;
            _repository = repository;
            _feCfg = feCfg;
        }
        public async Task<TournamentDto> Handle(GetTournamentQuery request, CancellationToken cancellationToken)
        {
            var tournament = await _repository.LoadAsync(request.TournamentId);

            var dto = new TournamentDto
            {
                Id = tournament.Id.Value,
                TournamentName = tournament.TournamentName,
                PlayerLimit = tournament.Limit.Limit,
                Players = tournament.Players.Select(p => new PlayerDto
                {
                    Name = p.Name,
                    Surname = p.Surname,
                    SelectedTeam = tournament.SelectedTeams.FirstOrDefault(st => st.Player == p)?.TeamName,
                    DrawnTeam = tournament.DrawTeams.FirstOrDefault(dt => dt.Player == p)?.TeamName
                }).ToList()
            };

            dto.QRCode = GenerateQrCodeForTournament(tournament.Id);

            return dto;
        }

        private string GenerateQrCodeForTournament(TournamentId id)
        {
            var feLink = "http://" + _feCfg.ConnectionString + "/player/register/" + id.Value;
            return _qrCodeCreator.Generate(feLink);
        }
    }
}
