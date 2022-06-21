using LCT.Application.Players.Commands;
using LCT.Application.Tournaments.Commands;
using LCT.Application.Tournaments.Queries;
using LCT.Core.Entites.Tournaments.Entities;
using LCT.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LCT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : BaseApiController
    {
        private readonly AggregateRepository _repo;
        public TournamentController(IMediator mediator, AggregateRepository repo) : base(mediator)
        {
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid Id)
            => Ok(await _mediator.Send(new GetTournamentQuery {  TournamentId = Id }));

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateTournamentCommand request)
        {
            var aggregate = await _repo.LoadAsync<Tournament>(Guid.NewGuid());
            return Ok(await _mediator.Send(request));
        }

        [HttpPost("assignPlayer")]
        public async Task<IActionResult> AssignPlayerToTournament(AssignPlayerToTournamentCommand request)
            => Ok(await _mediator.Send(request));

        [HttpGet("draw")]
        public async Task<IActionResult> DrawTeams(Guid tournamentId)
            => Ok(await _mediator.Send(new DrawTeamForPlayersQuery { TournamentId = tournamentId }));
    }
}
