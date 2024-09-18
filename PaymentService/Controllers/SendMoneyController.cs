using Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentService.Application.Request.Send.Command;

namespace PaymentService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendMoneyController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SendMoneyController> _logger;

        public SendMoneyController(IMediator mediator, ILogger<SendMoneyController> logger)
        {
            _mediator = mediator;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
        }


        /*
         * Author: Md. Ziaul Talukder
         * Date : 14/07/2024
         * Description: Send Money API
         */

        [HttpPost("[action]")]
        public async Task<IActionResult> Send(AddOrEditTransaction command)
        {
            if(command.TransactionAmount < 0)
            {
                return StatusCode(StatusCodes.Status400BadRequest, Result.Failure(new List<string> { "Invalid Amount " }));
            }

            _logger.LogInformation("requst received from this contact no: "+ command.ContactNo+ " amount :"+ command.TransactionAmount);
            var result = await _mediator.Send(command);
            _logger.LogInformation("send money request successfully processed");

            // notification send

            /*SendNotification sendNotification = new SendNotification();
            _logger.LogInformation("notification successfully processed");
            await _mediator.Send(sendNotification);*/

            return Ok(result);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> ActionResult()
        {
            return Ok();
        }
    }
}
