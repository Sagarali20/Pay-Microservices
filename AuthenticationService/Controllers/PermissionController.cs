using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Models;
using AuthenticationService.Utils;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;

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

        [HttpPost("[action]")]
        public async Task<IActionResult> SavePermission (AddGenericMap command)
        {

            try
            {
                var user = await _mediator.Send(command);


                return null;
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }

        }
    }
}
