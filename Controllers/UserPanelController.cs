using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class UserPanelController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public UserPanelController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("ProposePlace")]
        public IActionResult ProposePlace([FromBody] PlaceProposition place)
        {
            _context.PlacePropositions.Add(place);
            if(_context.SaveChanges()==1) return Ok("Proposition has been added!");
            return StatusCode(500, "Error while adding place proposal!");
        }
    }
}
