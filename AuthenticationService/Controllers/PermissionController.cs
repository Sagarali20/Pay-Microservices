using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Models;
using AuthenticationService.Utils;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using AuthenticationService.Application.Request.Login.Query;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PermissionController> _logger;
        public PermissionController(IMediator mediator, ILogger<PermissionController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /*
        * Author: Md.Sagar Ali
        * Date : 13/08/2024
        * Description: Save Permission to database.
        */

        [HttpPost("[action]")]
        public async Task<IActionResult> SavePermission (AddGenericMap command)
        {
            try
            {
                _logger.LogInformation("Save Permission requst received from Endpoint");
                var user = await _mediator.Send(command);
                _logger.LogInformation("Save Permission request successfully processed");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }

        /*
        * Author: Md.Sagar Ali
        * Date : 12/08/2024
        * Description: Get All Group list from database.
        */

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllGroup()
        {
            try
            {
                _logger.LogInformation("GetAllGroup requst received from Endpoint");
                return Ok(new {result=await _mediator.Send(new GetAllGroup())});

            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        /*
        * Author: Md.Sagar Ali
        * Date : 13/08/2024
        * Description: Save Group data to database.
        */

        [HttpPost("[action]")]
        public async Task<IActionResult> SaveGroup(AddorEditGroup command)
        {
            try
            {
                _logger.LogInformation("Save Group requst received from Endpoint");
                var Group = await _mediator.Send(command);
                _logger.LogInformation("Save Group request successfully processed");


                return Ok(Group);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        /*
        * Author: Md.Sagar Ali
        * Date : 13/08/2024
        * Description: Save Role data to database.
        */

        [HttpPost("[action]")]
        public async Task<IActionResult> SaveRole(AddOrEditRole command)
        {
            try
            {
                _logger.LogInformation("Save Role requst received from Endpoint");
                var Group = await _mediator.Send(command);
                _logger.LogInformation("Save Role request successfully processed");

                return Ok(Group);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        /*
        * Author: Md.Sagar Ali
        * Date : 12/08/2024
        * Description: Get All Role list from database.
        */

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                _logger.LogInformation("GetAllRole requst received from Endpoint");
                return Ok( new {result=await _mediator.Send(new GetAllRole())});
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        /*
        * Author: Md.Sagar Ali
        * Date : 12/08/2024
        * Description: Get All Permission list from database.
        */

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllPermission()
        {
            try
            {
                _logger.LogInformation("GetAll Permission requst received from Endpoint");
                return Ok(new {result=await _mediator.Send(new GetAllPermission())});
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
    }
}
