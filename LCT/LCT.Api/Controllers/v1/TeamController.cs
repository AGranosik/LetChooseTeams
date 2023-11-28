using Asp.Versioning;
using LCT.Api.Configuration.Models;
using LCT.Application.Teams.Commands;
using LCT.Application.Teams.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace LCT.Api.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion(1.0)]
    public class TeamController : BaseApiController
    {
        public TeamController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get available teams for tournament.")]
        [SwaggerResponse(200, "", typeof(TeamToSelectDto))]
        [SwaggerResponse(400, "Some error occured. Check logs with provided requestId.", typeof(ErrorResponseModel))]
        public async Task<IActionResult> Get(Guid TournamentId, CancellationToken cancellationToken)
            => Ok(await _mediator.Send(new GetTeamsQuery()
            {
                TournamentId = TournamentId
            }, cancellationToken));

        [HttpPost("select")]
        [SwaggerOperation(Summary = "Team selection by tournament player.")]
        [SwaggerResponse(200, "Team selected for a player.")]
        [SwaggerResponse(400, "Some error occured. Check logs with provided requestId.", typeof(ErrorResponseModel))]
        public async Task<IActionResult> SelectTeam(SelectTeamCommand request, CancellationToken cancellationToken)
        {
            await _mediator.Send(request, cancellationToken);
            return Ok();
        }
    }
}
