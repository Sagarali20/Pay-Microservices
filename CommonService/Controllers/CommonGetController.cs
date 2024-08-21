using CommonService.Application.Request.CommonGet.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommonService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommonGetController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CommonGetController> _logger;

        public CommonGetController(IMediator mediator, ILogger<CommonGetController> logger)
        {         
            _mediator = mediator;
            _logger = logger;
        }
        /*
        * Author: Md.Sagar Ali
        * Date : 18/08/2024
        * Description: Get All Account Type list from database.
        */
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllAccountType()
        {
            try
            {
                _logger.LogInformation("GetAll AccountType requst received from Endpoint");
                return Ok( new { result = await _mediator.Send(new GetAllType())});
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }
        /*
        * Author: Md.Sagar Ali
        * Date : 18/08/2024
        * Description: Get user balance from database.
        */
        [HttpPost("[action]")]
        public async Task<IActionResult> GetBalanceByAccountNumber(GetAccountBalance command)
        {
            try
            {
                _logger.LogInformation("GetAccount Balance requst received from Endpoint");
                return Ok(new {result= await _mediator.Send(command)});
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

    }
}
