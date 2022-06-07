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
    public class AdminActivitieController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public AdminActivitieController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("AddActivitie")]
        public IActionResult AddActivitie([FromBody] Activitie activitie)
        {
            _context.Activities.Add(activitie);
            if(_context.SaveChanges()!=1) return StatusCode(500, "Error while adding activitie!");
            return Ok();
        }
        
    }
}
