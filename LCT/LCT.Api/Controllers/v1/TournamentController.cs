using Asp.Versioning;
using LCT.Api.Configuration.Models;
using LCT.Application.Players.Commands;
using LCT.Application.Tournaments.Commands;
using LCT.Application.Tournaments.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LCT.Api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion(1.0)]
    public class TournamentController : BaseApiController
    {
        public TournamentController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get single tournament.")]
        [SwaggerResponse(200, "", typeof(TournamentDto))]
        [SwaggerResponse(400, "Some error occured. Check logs with provided requestId.", typeof(ErrorResponseModel))]
        public async Task<IActionResult> Get(Guid Id, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetTournamentQuery { TournamentId = Id }, cancellationToken));

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create tournament")]
        [SwaggerResponse(200, "Tournament created")]
        [SwaggerResponse(400, "Some error occured. Check logs with provided requestId.")]
        public async Task<IActionResult> Create(CreateTournamentCommand request, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(request, cancellationToken));

        [HttpPost("assignPlayer")]
        [SwaggerOperation(Summary = "Assign player to tournament.")]
        [SwaggerResponse(200, "Player assigned to tournament successfully.")]
        [SwaggerResponse(400, "Some error occured. Check logs with provided requestId.", typeof(ErrorResponseModel))]
        public async Task<IActionResult> AssignPlayerToTournament(AssignPlayerToTournamentCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            return Ok();
        }

        [HttpGet("draw")]
        [SwaggerOperation(Summary = "Draw team for players when all seats are taken.")]
        [SwaggerResponse(200, "Teams drawn successfully.", typeof(List<DrawnTeamDto>))]
        [SwaggerResponse(400, "Some error occured. Check logs with provided requestId.", typeof(ErrorResponseModel))]
        public async Task<IActionResult> DrawTeams(Guid tournamentId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new DrawTeamForPlayersQuery { TournamentId = tournamentId }, cancellationToken));
    }
}
