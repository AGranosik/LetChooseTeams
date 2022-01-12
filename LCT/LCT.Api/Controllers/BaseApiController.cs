using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LCT.Api.Controllers
{
    public abstract class BaseApiController : ControllerBase
    {
        protected readonly IMediator _mediator;

        protected BaseApiController(IMediator mediator)
            => _mediator = mediator;
    }
}
