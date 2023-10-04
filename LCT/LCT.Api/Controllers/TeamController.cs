using LCT.Application.Teams.Commands;
using LCT.Application.Teams.Queries;
using LCT.Application.Tournaments.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Annotations;

namespace LCT.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : BaseApiController
    {
        public TeamController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get available teams for tournament.")]
        [SwaggerResponse(200, "", typeof(TeamToSelectDto))]
        [SwaggerResponse(400, "Some error occured. Check logs with provided requestId.")]
        public async Task<IActionResult> Get(Guid TournamentId)
            => Ok(await _mediator.Send(new GetTeamsQuery()
            {
                TournamentId = TournamentId
            }));

        [HttpPost("select")]
        [SwaggerOperation(Summary = "Team selection by tournament player.")]
        [SwaggerResponse(200, "Team selected for a player.")]
        [SwaggerResponse(400, "Some error occured. Check logs with provided requestId.")]
        public async Task<IActionResult> SelectTeam(SelectTeamCommand request)
        {
            await _mediator.Send(request);
            return Ok();
        }
    }
}
