﻿using LCT.Application.Players.Commands;
using LCT.Application.Tournaments.Commands;
using LCT.Application.Tournaments.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LCT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : BaseApiController
    {
        public TournamentController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid Id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetTournamentQuery {  TournamentId = Id }, cancellationToken));

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateTournamentCommand request)
            => Ok(await _mediator.Send(request));

        [HttpPost("assignPlayer")]
        public async Task<IActionResult> AssignPlayerToTournament(AssignPlayerToTournamentCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }

        [HttpGet("draw")]
        public async Task<IActionResult> DrawTeams(Guid tournamentId)
            => Ok(await _mediator.Send(new DrawTeamForPlayersQuery { TournamentId = tournamentId }));
    }
}
