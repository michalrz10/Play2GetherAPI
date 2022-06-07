using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserPremiumController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public UserPremiumController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet("IsPremium")]
        public IActionResult IsPremium()
        {
            User user = _context.Users.Find(long.Parse((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            if (user != null)
            {
                if (user.Role.Equals("Admin")) return Ok();
                Premium premium = _context.Premiums.Where(p => p.UserId.Equals(user.UserId)).OrderBy(p => p.StartDate).FirstOrDefault();
                if(premium != null)
                {
                    if(DateTime.Now < premium.StartDate.AddDays(premium.Days))
                    {
                        return Ok();
                    }
                    return BadRequest("Date have expired");
                }
                return BadRequest("Premium not found");
            }
            return NotFound("User not found");
        }

        [HttpGet("IsPremium/{id}")]
        public IActionResult IsPremium(long id)
        {
            User user = _context.Users.Find(id);
            if (user != null)
            {
                if(user.Role.Equals("Admin")) return Ok();
                Premium premium = _context.Premiums.Where(p => p.UserId.Equals(user.UserId)).OrderBy(p => p.StartDate).FirstOrDefault();
                if (premium != null)
                {
                    if (DateTime.Now < premium.StartDate.AddDays(premium.Days))
                    {
                        return Ok();
                    }
                    return BadRequest("Date have expired");
                }
                return BadRequest("Premium not found");
            }
            return NotFound("User not found");
        }

        [HttpPost("AddPremium")]
        public IActionResult AddPremium([FromBody] Premium premium)
        {
            premium.StartDate = DateTime.Now;
            User user = _context.Users.Find(long.Parse((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value));
            if (user != null)
            {
                if (user.Role.Equals("Admin")) return Ok();
                Premium prevPremium = _context.Premiums.Where(p => p.UserId.Equals(user.UserId)).OrderBy(p => p.StartDate).FirstOrDefault();
                if (prevPremium != null)
                {
                    if (DateTime.Now < premium.StartDate.AddDays(premium.Days))
                    {
                        premium.StartDate.AddMinutes(DateTime.Now.Minute - prevPremium.StartDate.Minute - prevPremium.Days * 24 * 60);
                    }
                }
                premium.UserId = user.UserId;
                _context.Premiums.Add(premium);
                if (_context.SaveChanges() == 1) return Ok();
                return StatusCode(500, "Error while adding premium!");
            }
            return NotFound("User not found");
        }
    }
}
