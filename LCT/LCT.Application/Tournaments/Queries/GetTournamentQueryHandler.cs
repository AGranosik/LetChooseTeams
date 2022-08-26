﻿using LCT.Application.Common;
using LCT.Core.Shared.Exceptions;
using LCT.Domain.Aggregates.TournamentAggregate.Entities;
using LCT.Infrastructure.Repositories;
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
        public GetTournamentQueryHandler(IQRCodeCreator qRCodeCreator, IRepository<Tournament> repository)
        {
            _qrCodeCreator = qRCodeCreator;
            _repository = repository;
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
                    SelectedTeam = tournament.SelectedTeams.Where(st => st.Player == p).Select(st => st.TeamName).FirstOrDefault(),
                    DrawnTeam = tournament.DrawTeams.Where(dt => dt.Player == p).Select(dt => dt.TeamName).FirstOrDefault()
                }).ToList()
            };

            // move it to function

            if (tournament == null)
                throw new EntityDoesNotExist(nameof(Tournament));

            var feLink = "http://" + IpAdressProvider.GetHostAdress() + ":3000/player/register/" + request.TournamentId;
            dto.QRCode = _qrCodeCreator.Generate(feLink);

            return dto;
        }
    }
}
