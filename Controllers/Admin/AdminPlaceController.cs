using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Play2GetherAPI.ControllerModels;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPlaceController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public AdminPlaceController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("AddActivitieToPlace/{placeId}/{activitieId}")]
        public IActionResult AddActivitieToPlace(long placeId, long activitieId)
        {
            var place = _context.Places.FirstOrDefault(a => a.PlaceId == placeId);
            var activitie = _context.Activities.FirstOrDefault(a => a.ActivitieId == activitieId);
            if (place == null || activitie == null) return BadRequest("Wrong id");
            if (_context.PlaceActivities.FirstOrDefault(pa => pa.PlaceId == place.PlaceId && pa.ActivitieId == activitie.ActivitieId) != null) return BadRequest("Activitie already added to place!");
            var pa = new PlaceActivitie()
            {
                PlaceId = placeId,
                ActivitieId = activitieId,
                //PlaceA = place,
                //ActivitieP = activitie
            };
            _context.PlaceActivities.Add(pa);
            if (_context.SaveChanges() == 1) return Ok("Added activitie to palce");
            return StatusCode(500, "Error while adding activitie to place!");
        }

        [HttpPost("AddPlace")]
        public IActionResult AddPlace([FromBody] Place place)
        {
            place.ImageUrl = "http://87.205.116.41:5000/api/Basic/images/defaultplace.jpg";
            _context.Places.Add(place);
            if (_context.SaveChanges() == 1) return Ok("Place has been added!");
            return StatusCode(500, "Error while adding place!");
        }

        [HttpPost("UploadImage/{id}")]
        public IActionResult UploadImage(IFormFile file, long id)
        {
            if (file == null) return BadRequest("empty file");
            var filePath = "images/" + System.IO.Path.GetRandomFileName() + ".jpg";
            if (file.Length > 0)
            {

                while (System.IO.File.Exists(filePath)) filePath = "images/" + System.IO.Path.GetRandomFileName() + ".jpg";
                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                var img = new Bitmap(System.Drawing.Image.FromStream(memoryStream));
                using (var output = System.IO.File.Open(filePath, FileMode.Create))
                {
                    var qualityParamId = Encoder.Quality;
                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(qualityParamId, 25L);
                    img.Save(output, ImageCodecInfo.GetImageEncoders().FirstOrDefault(ie => ie.MimeType == "image/jpeg"), encoderParameters);
                }

            }
            else return BadRequest("empty file");
            _context.Places.Find(id).ImageUrl = "http://87.205.116.41:5000/api/Basic/" + filePath;
            if (_context.SaveChanges() != 1) return StatusCode(500, "Could not save url to database");
            return Ok();

        }
    }
}
