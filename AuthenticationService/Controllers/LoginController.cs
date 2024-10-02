using AuthenticationService.Application.Request.Login.Command;
using AuthenticationService.Utils;
using JwtAuthenticationManager.Models;
using JwtAuthenticationManager;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthenticationService.Models;
using Consul;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AuthenticationService.Helpers.Interface;
using AuthenticationService.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using AuthenticationService.Application.Request.Login.Query;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;
using Newtonsoft.Json.Linq;

namespace AuthenticationService.Controllers
{
    [Route("api/Authentication")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly IMediator _mediator;
        private readonly ILogger<LoginController> _logger;
        private readonly IWebHostEnvironment _environment;
        // Allowed image file types
        private readonly string[] permittedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        // Max file size allowed 5 MB 
        private const long maxFileSize = 5 * 1024 * 1024;

        public LoginController(IMediator mediator, JwtTokenHandler jwtTokenHandler, ILogger<LoginController> logger, IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _jwtTokenHandler = jwtTokenHandler;
            _logger = logger;
            _environment = environment;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> SaveUser(AddOrEditUser command)
        {
            _logger.LogInformation("User save requst received from Endpoint");
            var result = await _mediator.Send(command);
            _logger.LogInformation("User save request successfully processed");

            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Updateuser(Updateuser command)
        {

            var result = await _mediator.Send(command);
            return Ok(result);
        }


        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginUser command)
        {
            try
            {
                _logger.LogInformation("User Validation requst received from Endpoint");

                command.Password = MD5Encryption.GetMD5HashData(command.Password);
                AuthenticationResponse authenticationResponse = new AuthenticationResponse();
                User user = await _mediator.Send(command);
                if (user is not null)
                {
                    if (user.tx_password == command.Password)
                    {
                        authenticationResponse = _jwtTokenHandler.GenerateJwtToken(user.id_user_key, user.tx_mobile_no, 0, "");
                        var res = new
                        {
                            result = true,
                            userFullName = user.tx_first_name + " " + user.tx_last_name,
                            token = authenticationResponse.JwtToken,
                            permission = user.Permission
                        };
                        _logger.LogInformation("User Validation request successfully processed");

                        return Ok(res);

                    }
                    _logger.LogInformation("User Validation request faild because of Invalid password");

                    return BadRequest(new { result = false, message = "Invalid password." });
                }
                _logger.LogInformation("User Validation request faild because of User not found!");

                return NotFound(new { result = false, message = "User not found!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Ok(ex.Message);
            }

        }
        [HttpGet("[action]")]
        public async Task<IActionResult> Refresh()
        {
            _logger.LogInformation("Refresh token requst received from Endpoint");
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (token is null)
            {
                _logger.LogWarning("invalid client request for token");
                return BadRequest("invalid client request");
            }
            try
            {
                var pricipal = GetPrincipleFromExpiredToken(token);

                var result = await _mediator.Send(new LoginUser(pricipal.Identity.Name.ToString(), ""));
                if (result is not null)
                {
                    _logger.LogInformation("User token request successfully processed");
                    return Ok(new { token = _jwtTokenHandler.GenerateJwtToken(result.id_user_key, result.tx_mobile_no, 0, "").JwtToken.ToString() });

                }
                _logger.LogWarning("invalid client request for token");

                return BadRequest(new { token = "Invalid user!" });
                //var newAccessToken = createJwt(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Ok(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadImage([FromForm] IFormFile image)
        {

            bool result = false;
            string path = "";
            // Validate file extension
            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!permittedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file format. Only JPG, JPEG, PNG, and GIF are allowed.");
            }

            // Validate file size
            if (image.Length > maxFileSize)
            {
                return BadRequest($"File size exceeds the limit of {maxFileSize / (1024 * 1024)} MB.");
            }

            var date = String.Format(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day);
            if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
            {
                _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                path = _environment.WebRootPath + "\\Files\\" + "\\" + "UserDocument" + "\\" + "\\" + "Images" + "\\" + "\\" + date + "\\";
            }
            else
            {
                path = _environment.WebRootPath + "\\Files\\" + "\\" + "UserDocument" + "\\" + "\\" + "Images" + "\\" + "\\" + date + "\\";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //var fileName = imageFile.FileName;
            //Random _random = new Random();
            var fileName = Path.GetFileNameWithoutExtension(image.FileName) + "-" + Guid.NewGuid() + Path.GetExtension(image.FileName);
            using (FileStream fileStream = System.IO.File.Create(path + fileName))
            {
                image.CopyTo(fileStream);
                fileStream.Flush();
                path = Path.Combine(string.Format("/Files/" + "UserDocument" + "/Images" + "/{0}/{1}", date, fileName));
            }
            return Ok(new { image.FileName, path });
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {

            string[] permittedExtensions = { ".doc", ".docx", ".pdf", ".txt" };
            bool result = false;
            string path = "";
            // Validate file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!permittedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file format. Only doc, docx, pdf, and txt are allowed.");
            }

            var date = String.Format(DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day);
            if (string.IsNullOrWhiteSpace(_environment.WebRootPath))
            {
                _environment.WebRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                path = _environment.WebRootPath + "\\Files\\" + "\\" + "UserDocument" + "\\" + "\\" + "Files" + "\\" + "\\" + date + "\\";
            }
            else
            {
                path = _environment.WebRootPath + "\\Files\\" + "\\" + "UserDocument" + "\\" + "\\" + "Files" + "\\" + "\\" + date + "\\";
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //var fileName = imageFile.FileName;
            //Random _random = new Random();
            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "-" + Guid.NewGuid() + Path.GetExtension(file.FileName);
            using (FileStream fileStream = System.IO.File.Create(path + fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
                path = Path.Combine(string.Format("/Files/" + "UserDocument" + "/Files" + "/{0}/{1}", date, fileName));
            }
            return Ok(new { file.FileName, path });
        }

        [HttpPost("[action]")]

        public async Task<IActionResult> Resetpassword(ResetPassword command)
        {
            try
            {
                if (!string.IsNullOrEmpty(command.Token))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    tokenHandler.ValidateToken(command.Token, new TokenValidationParameters()
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtTokenHandler.JWT_SECURITY_KEY)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                    }, out SecurityToken validatedToken);
                }

                var result = await _mediator.Send(command);
                return Ok(result);

            }
            catch (Exception ex)
            {
                var sss = new
                {
                    title = "something wrong",
                    success = false,
                    errorMessage = ex.Message,
                    statusCode = StatusCodes.Status500InternalServerError,
                };
                return Ok(sss);
            }
            
        }
        
        
        [HttpPost("[action]")]
        public async Task<IActionResult> SaveUserDocument(SaveUserDocument command)
        {
            _logger.LogInformation("User Document save requst received from Endpoint");
            var result = await _mediator.Send(command);
            _logger.LogInformation("User Document save request successfully processed");

            return Ok(result);
        }
        /*
        * Author: Md.Sagar Ali
        * Date : 08/31/2024
        * Description: Get All Account Type list from database.
        */
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllDocumentType()
        {
            try
            {
                _logger.LogInformation("GetAll Document type requst received from Endpoint");
                return Ok(new { result = await _mediator.Send(new GetAllDocumentType()) });
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }

        private ClaimsPrincipal GetPrincipleFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(JwtTokenHandler.JWT_SECURITY_KEY);

            var tokenValidationParameter = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenhandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenhandler.ValidateToken(token, tokenValidationParameter, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("this is invalid token");
            return principal;

        }

        /*
         * author: ziaul talukder
         * date : 09/04/2024
         * description: otp send service
         */
        [HttpPost("[action]")]
        public async Task<IActionResult> SendOTP(SendOtp command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /*
         * author: ziaul talukder
         * date : 09/04/2024
         * description: otp send service
         */
        [HttpPost("[action]")]
        public async Task<IActionResult> ValidateOTP(ValidateOtp command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogOut()
        {

            return Ok("ok");
        }
    }
}
