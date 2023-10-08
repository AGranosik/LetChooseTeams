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
        private readonly IAggregateRepository<Tournament> _repository;
        public GetTournamentQueryHandler(IAggregateRepository<Tournament> repository)
        {
            _repository = repository;
        }
        public async Task<TournamentDto> Handle(GetTournamentQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var tournament = await _repository.LoadAsync(request.TournamentId, cancellationToken);
            return new TournamentDto
            {
                Id = tournament.Id.Value,
                TournamentName = tournament.TournamentName,
                PlayerLimit = tournament.Limit.Limit,
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
    }
}
