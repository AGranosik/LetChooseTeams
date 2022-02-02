using LCT.Application.Players.Commands;
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
        private readonly IElasticClient _client
        public TournamentController(IMediator mediator, IElasticClient client) : base(mediator)
        {
            _client = client;
            _client
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateTournamentCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("assignPlayer")]
        public async Task<IActionResult> AssignPlayerToTournament(AssignPlayerToTournamentCommand request)
        {
            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
