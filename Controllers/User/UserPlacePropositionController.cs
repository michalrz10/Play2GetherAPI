using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserPlacePropositionController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public UserPlacePropositionController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("ProposePlace")]
        public IActionResult ProposePlace([FromBody] PlaceProposition place)
        {
            place.ImageUrl = "http://87.205.116.41:5000/api/Basic/images/defaultplace.jpg";
            _context.PlacePropositions.Add(place);
            if(_context.SaveChanges()==1) return Ok("Proposition has been added!");
            return StatusCode(500, "Error while adding place proposal!");
        }

    }
}
