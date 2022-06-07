using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserUserController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public UserUserController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet("GetUser")]
        public ActionResult<User> GetUser()
        {
            var id = long.Parse((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if(user != null)
            {
                Premium premium = _context.Premiums.Where(p => p.UserId.Equals(user.UserId)).OrderBy(p => p.StartDate).FirstOrDefault();
                if (premium != null)
                {
                    if (DateTime.Now < premium.StartDate.AddDays(premium.Days))
                    {
                        user.isPremium = true;
                    }
                    user.isPremium = false;
                }
                user.isPremium = false;
                if (user.Role.Equals("Admin")) user.isPremium = true;
            }
            return user;
        }

        [HttpPut("EditUser")]
        public IActionResult EditUser([FromBody] User editedUser)
        {
            var id = long.Parse((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                if(editedUser.Age>0) user.Age=editedUser.Age;
                user.Name=editedUser.Name;
                user.SurName=editedUser.SurName;
                if (_context.SaveChanges() == 1) return Ok();
                return StatusCode(500, "Could not save edited user");
            }
            return BadRequest("Wrong id!");
        }

        [HttpGet("GetUser/{id}")]
        public ActionResult<User> GetUserId(long id)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                Premium premium = _context.Premiums.Where(p => p.UserId.Equals(user.UserId)).OrderBy(p => p.StartDate).FirstOrDefault();
                if (premium != null)
                {
                    if (DateTime.Now < premium.StartDate.AddDays(premium.Days))
                    {
                        user.isPremium = true;
                    }
                    user.isPremium = false;
                }
                user.isPremium = false;
                if (user.Role.Equals("Admin")) user.isPremium = true;
            }
            return user;
        }

        [HttpPost("UploadImage")]
        public IActionResult UploadImage(IFormFile file)
        {
            if (file == null) return BadRequest("empty file");
            var id = long.Parse((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var filePath = "images/" + System.IO.Path.GetRandomFileName() + ".jpg";
            if (file.Length > 0)
            {
                
                while(System.IO.File.Exists(filePath)) filePath = "images/" + System.IO.Path.GetRandomFileName() + ".jpg";
                var memoryStream = new MemoryStream();
                file.CopyTo(memoryStream);
                var img = new Bitmap(System.Drawing.Image.FromStream(memoryStream));
                var resized = new Bitmap(128, 128);
                using (var graphics = Graphics.FromImage(resized))
                {
                    graphics.DrawImage(img, 0, 0, 128, 128);
                    using (var output = System.IO.File.Open(filePath, FileMode.Create))
                    {
                        var qualityParamId = Encoder.Quality;
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(qualityParamId, 25L);
                        resized.Save(output, ImageCodecInfo.GetImageEncoders().FirstOrDefault(ie=>ie.MimeType=="image/jpeg"), encoderParameters);
                    }
                }

            }
            else return BadRequest("empty file");
            _context.Users.Find(id).ImageUrl = "http://87.205.116.41:5000/api/Basic/" + filePath;
            if(_context.SaveChanges()!=1) return StatusCode(500, "Could not save url to database");
            return Ok();

        }

    }
}
