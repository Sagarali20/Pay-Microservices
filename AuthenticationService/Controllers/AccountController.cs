using AuthenticationService.Models;
using AuthenticationService.Utils;
using JwtAuthenticationManager;
using JwtAuthenticationManager.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JwtTokenHandler _jwtTokenHandler;
        private readonly IWebHostEnvironment _environment;


        public AccountController(JwtTokenHandler jwtTokenHandler, IWebHostEnvironment environment)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _environment = environment; 
        }

        // Dictionary to simulate a database of users with their corresponding image IDs
        private static readonly List<User> users = new List<User>
        {
            new User { id_user_key = 1, tx_first_name = "John Doe" },
            new User { id_user_key = 2, tx_first_name = "Jane Smith" }
        };

        // Image file paths mapped to identifiers (for demo purposes)
        private readonly Dictionary<string, string> imageMap = new Dictionary<string, string>
        {
            { "img1", "wwwroot/Files/Documentfile/2024-8-25/handeler.JPG" },
            { "img2", "wwwroot/Files/Documentfile/2024-8-25/anotherImage.JPG" }
        };

        // GET: api/users
        [HttpGet("[action]")]
        public IActionResult GetUsers()
        {
            // Return the list of users with masked image URLs
            var usersWithMaskedImages = users.Select(user => new
            {
                user.id_user_key,
                user.tx_first_name,
                // Construct a masked image URL using the custom route
                ImageUrl = Url.Action("GetImageById", "Auth", new { id = user.id_user_key }, Request.Scheme)
            });

            return Ok(usersWithMaskedImages);
        }



        [HttpPost]
        public ActionResult<AuthenticationResponse?> Authenticate([FromBody] AuthenticationRequest authenticationRequest)
        {
            //string password = MD5Encryption.GetMD5HashData(authenticationRequest.Password);
            //var authenticationResponse = _jwtTokenHandler.GenerateJwtToken(authenticationRequest);
            //if (authenticationResponse == null) return Unauthorized();
            return null;
        }
        // GET: api/images/{id}
        [HttpGet]
        //public IActionResult GetImageById()
        //{

        //    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Documentfile/2024-8-25/handeler.JPG");
        //    // Check if file exists
        //    if (!System.IO.File.Exists(fullPath))
        //    {
        //        return NotFound("File not found");
        //    }

        //    // Serve the image as a file
        //    var image = System.IO.File.OpenRead(fullPath);
        //    return File(image, "image/jpeg");
        //}

        [HttpPost("[action]")]
        public async Task<IActionResult> getBase64()
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Files/Documentfile/2024-8-25/handeler.JPG");
            // Check if file exists
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("File not found");
            }
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("Image not found");
            }

            byte[] imageBytes = await System.IO.File.ReadAllBytesAsync(fullPath);

            string base64String = Convert.ToBase64String(imageBytes);

            return Ok(new { base64String = base64String });

        }

    }
}
