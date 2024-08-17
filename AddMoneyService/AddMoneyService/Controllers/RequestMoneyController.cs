using AddMoneyService.Application.RequestMoney.Command;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AddMoneyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestMoneyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RequestMoneyController> _logger;
        public RequestMoneyController(IMediator mediator, ILogger<RequestMoneyController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> RequestMoney(AddRequestMoney command)
        {
            _logger.LogInformation("request receive from controller");
            if (command.DccAmount < 0)
            {
                _logger.LogInformation("user input negative amount : "+ command.DccAmount);
                return Ok("Invalid Amount");
            }

            _logger.LogInformation("requst received from this sender no: " + command.TxSenderPhone+ " amount :" + command.DccAmount + " receiver contact no : "+ command.TxReceiverPhone + " Charge Amount : "+ command.DccChargeAmount);

            var result = await _mediator.Send(command);
            
            _logger.LogInformation("request money successfully processed");

            return Ok(result);
        }
    }
}
