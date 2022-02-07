﻿using LCT.Application.Players.Commands;
using LCT.Application.Tournaments;
using LCT.Application.Tournaments.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nest;

namespace LCT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentController : BaseApiController
    {
        public TournamentController(IMediator mediator) : base(mediator)
        {
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateTournamentCommand request)
            => Ok(await _mediator.Send(request));

        [HttpPost("assignPlayer")]
        public async Task<IActionResult> AssignPlayerToTournament(AssignPlayerToTournamentCommand request)
            => Ok(await _mediator.Send(request));
    }
}
