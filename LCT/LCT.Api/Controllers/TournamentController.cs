using LCT.Application.Players.Commands;
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

        [HttpGet("id")]
        public async Task<IActionResult> Get(Guid Id)
            => Ok(await _mediator.Send(new GetTournamentQuery {  TournamentId = Id }));

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateTournamentCommand request)
            => Ok(await _mediator.Send(request));

        [HttpPost("assignPlayer")]
        public async Task<IActionResult> AssignPlayerToTournament(AssignPlayerToTournamentCommand request)
            => Ok(await _mediator.Send(request));
    }
}
