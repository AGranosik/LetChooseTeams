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
        public async Task<IActionResult> Get()
            => Ok(await _mediator.Send(new GetTeamsQuery()));

        //[HttpPost]
    }
}
