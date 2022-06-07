using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Play2GetherAPI.ControllerModels;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasicController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public BasicController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet("IsWorking")]
        public IActionResult IsWorking()
        {
            return Ok();
        }

        [Authorize]
        [HttpGet("GetRole")]
        public IActionResult GetRole()
        {
            return Ok((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value);
        }

        [HttpGet("images/{url}")]
        [AllowAnonymous]
        public IActionResult GetImage(string url)
        {
            if (url.Equals("null")) url = "default.jpg";
            url = "images/" + url;
            if (!System.IO.File.Exists(url)) return BadRequest("Wrong Url");
            System.Byte[] b = System.IO.File.ReadAllBytes(url);
            return File(b, "image/jpeg");
        }
    }
}
