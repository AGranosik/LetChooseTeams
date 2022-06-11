using LCT.Application.Teams.Commands;
using LCT.Application.Teams.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Get(Guid TournamentId)
            => Ok(await _mediator.Send(new GetTeamsQuery()
            {
                TournamentId = TournamentId
            }));

        [HttpPost("select")]
        public async Task<IActionResult> SelectTeam(SelectTeamCommand request)
            => Ok(await _mediator.Send(request));
    }
}
