using Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RemittanceService.Application.Request.Ramittance.Command;

namespace RemittanceService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RamittanceController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RamittanceController> _logger;

        public RamittanceController(IMediator mediator, ILogger<RamittanceController> logger)
        {
            _mediator = mediator;
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger;
        }


        /*
         * Author: Md. Ziaul Talukder
         * Date : 14/07/2024
         * Description: remittance API
         */

        [HttpPost("[action]")]
        public async Task<IActionResult> Ramittance(RamittanceInformation command)
        {
            if(command.CustomerAmount < 0)
            {
                _logger.LogInformation("invalid amount from user : "+ command.CustomerAmount);
                return StatusCode(StatusCodes.Status400BadRequest, Result.Failure(new List<string> { "Invalid Amount " }));
            }
            else if(command.BeneficiaryAmount < 0)
            {
                _logger.LogInformation("invalid amount from user : " + command.BeneficiaryAmount);
                return StatusCode(StatusCodes.Status400BadRequest, Result.Failure(new List<string> { "Invalid Amount " }));
            }

            _logger.LogInformation("request receive from ");
            var result = await _mediator.Send(command);
            _logger.LogInformation("remittance send success");

            return Ok(result);
        }
    }
}
