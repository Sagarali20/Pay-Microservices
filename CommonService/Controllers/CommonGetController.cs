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

        public CommonGetController(IMediator mediator)
        {         
            _mediator = mediator;
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
        public async Task<IActionResult> GetBalanceByAccountNumber(GetAccountBalancec command)
        {
            try
            {
                return Ok(new {result= await _mediator.Send(command)});
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

    }
}
