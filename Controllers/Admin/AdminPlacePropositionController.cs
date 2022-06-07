using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Play2GetherAPI.ControllerModels;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPlacePropositionController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public AdminPlacePropositionController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("ProposePlaceDecision")]
        public IActionResult ProposePlaceDecision([FromBody] PlaceDecision placeDec)
        {
            var placeprop = _context.PlacePropositions.Find((long)placeDec.Id);
            if (placeprop == null) return BadRequest("No place proposition with that id!");
            if(placeDec.Decision)
            {
                Place place = new Place
                {
                    Latitude = placeprop.Latitude,
                    Longitude = placeprop.Longitude,
                    ImageUrl = "api/Basic/images/defaultplace.jpg",
                    Name = placeprop.Name
                };
                _context.Places.Add(place);
                _context.PlacePropositions.Remove(placeprop);
                _context.SaveChanges();
            }
            else
            {
                _context.PlacePropositions.Remove(placeprop);
                _context.SaveChanges();
            }
            return Ok();
        }

        [HttpGet("PlacePropositions")]
        public async Task<ActionResult<IEnumerable<PlaceProposition>>> PlacePropositions()
        {
            return await _context.PlacePropositions.ToListAsync();
        }
    }
}
