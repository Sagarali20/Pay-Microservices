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

        public PermissionController(IMediator mediator)
        {
            _mediator = mediator;
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
                var user = await _mediator.Send(command);
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

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllGroup()
        {
            try
            {
                return Ok(await _mediator.Send(new GetAllGroup()));
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
                var Group = await _mediator.Send(command);

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
                var Group = await _mediator.Send(command);

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

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                return Ok(await _mediator.Send(new GetAllRole()));
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

        [HttpPost("[action]")]
        public async Task<IActionResult> GetAllPermission()
        {
            try
            {
                return Ok(await _mediator.Send(new GetAllPermission()));
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
    }
}
