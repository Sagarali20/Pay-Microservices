using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SendMoneyService.Application.Request.Send.Command;

namespace SendMoneyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMoneyController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SendMoneyController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Send(AddOrEditTransaction command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
