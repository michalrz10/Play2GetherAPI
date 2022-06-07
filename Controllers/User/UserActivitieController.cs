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
    public class UserActivitieController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public UserActivitieController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet("GetActivities")]
        public async Task<ActionResult<IEnumerable<Activitie>>> GetActivities()
        {
            return await _context.Activities.ToListAsync();
        }

    }
}
