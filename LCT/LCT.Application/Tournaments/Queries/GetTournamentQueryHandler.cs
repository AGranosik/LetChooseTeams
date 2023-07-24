using System.Text;
using LCT.Application.Common;
using LCT.Application.Common.Configs;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Domain.Common.Interfaces;
using MediatR;

namespace LCT.Application.Tournaments.Queries
{
    public class TournamentDto
    {
        public Guid Id { get; set; }
        public string TournamentName { get; set; }
        public List<PlayerDto> Players { get; set; }
        public string QRCode { get; set; }
        public int PlayerLimit { get; set; }
        public int Version { get; set; }
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
        private readonly IAggregateRepository<Tournament> _repository;
        private readonly FrontendConfiguration _feCfg;
        public GetTournamentQueryHandler(IQRCodeCreator qRCodeCreator, IAggregateRepository<Tournament> repository, FrontendConfiguration feCfg)
        {
            _qrCodeCreator = qRCodeCreator;
            _repository = repository;
            _feCfg = feCfg;
        }
        public async Task<TournamentDto> Handle(GetTournamentQuery request, CancellationToken cancellationToken)
        {
            var generationQrCodeTask = GenerateQrCodeForTournament(request.TournamentId);
            var tournament = await _repository.LoadAsync(request.TournamentId);
            return new TournamentDto
            {
                Id = tournament.Id.Value,
                TournamentName = tournament.TournamentName,
                PlayerLimit = tournament.Limit.Limit,
                QRCode = await generationQrCodeTask,
                Version = tournament.Version,
                Players = tournament.Players.Select(p => new PlayerDto
                {
                    Name = p.Name,
                    Surname = p.Surname,
                    SelectedTeam = tournament.SelectedTeams.FirstOrDefault(st => st.Player == p)?.TeamName,
                    DrawnTeam = tournament.DrawTeams.FirstOrDefault(dt => dt.Player == p)?.TeamName
                }).ToList()
            };
        }

        private async Task<string> GenerateQrCodeForTournament(Guid id)
        {
            return await Task.Run(() =>
            {
                var stringBuilder = new StringBuilder("http://");
                stringBuilder.Append(_feCfg.ConnectionString);
                stringBuilder.Append("/player/register/");
                stringBuilder.Append(id);

                return _qrCodeCreator.Generate(stringBuilder.ToString());
            });
        }
    }
}
