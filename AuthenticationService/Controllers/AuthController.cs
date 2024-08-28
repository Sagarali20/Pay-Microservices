using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthenticationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Dictionary<string, string> imageMap = new Dictionary<string, string>
        {
            { "1", "wwwroot/Files/Documentfile/2024-8-25/handeler.JPG" },
            { "2", "wwwroot/Files/Userfile/2024-8-25/main-qimg-7a210307ecb15f425053ab2e45e404b4-lq-33387ebd-c103-4fe3-aa49-f955a7087c60.jfif" }
        };

        // GET: api/images/{id}
        [HttpGet("{id}")]
        public IActionResult GetImageById(string id)
        {
            // Lookup the image path by its identifier
            if (!imageMap.TryGetValue(id, out string filePath))
            {
                return NotFound("Image not found");
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), filePath);

            // Check if the file exists
            if (!System.IO.File.Exists(fullPath))
            {
                return NotFound("File not found");
            }

            // Serve the image
            var image = System.IO.File.OpenRead(fullPath);
            return File(image, "image/jpeg");
        }


    }
}
