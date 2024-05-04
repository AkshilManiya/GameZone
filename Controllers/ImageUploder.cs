using Microsoft.AspNetCore.Mvc;

namespace GameZoneManagment.Controllers
{
    [Route("Upload")]
    public class ImageUploder : Controller
    {
        private readonly string _uploadPathClub = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", "Club");
        private readonly string _uploadPathGame = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", "Game");
        private readonly string _uploadPathUser = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Assets", "User");

        [HttpPost("Club")]
        public async Task<IActionResult> UploadImageClub([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file uploaded");
            }

            string fileName = image.FileName;
            string filePath = Path.Combine(_uploadPathClub, fileName);

            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("An image with the same name already exists. Please rename your image and try again.");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return Ok(new { success = true, message = "Image uploaded successfully!" });
        }

        [HttpPost("Game")]
        public async Task<IActionResult> UploadImageGame([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file uploaded");
            }

            string fileName = image.FileName;
            string filePath = Path.Combine(_uploadPathGame, fileName);

            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("An image with the same name already exists. Please rename your image and try again.");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return Ok(new { success = true, message = "Image uploaded successfully!" });
        }

        [HttpPost("User")]
        public async Task<IActionResult> UploadImageUser([FromForm] IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No image file uploaded");
            }

            string fileName = image.FileName;
            string filePath = Path.Combine(_uploadPathUser, fileName);

            if (System.IO.File.Exists(filePath))
            {
                return BadRequest("An image with the same name already exists. Please rename your image and try again.");
            }

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }
            return Ok(new { success = true, message = "Image uploaded successfully!" });
        }
    }
}
