using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserPlaceController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public UserPlaceController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet("GetPlaces")]
        public ActionResult<IEnumerable<Place>> GetPlaces()
        {
            var places = _context.Places.ToList();
            foreach (var place in places)
            {
                place.PlaceActivities= _context.PlaceActivities.Where(pa => pa.PlaceId == place.PlaceId).AsNoTracking().ToList();
                foreach (var pa in place.PlaceActivities)
                {
                    pa.ActivitieP = _context.Activities.FirstOrDefault(a => a.ActivitieId == pa.ActivitieId);
                }
            }
            return places;
        }

        [HttpGet("GetPlace/{id}")]
        public ActionResult<Place> GetPlacee(long id)
        {
            var place = _context.Places.FirstOrDefault(p=>p.PlaceId==id);
            if(place != null)
            {
                place.PlaceActivities = _context.PlaceActivities.Where(pa => pa.PlaceId == place.PlaceId).AsNoTracking().ToList();
                foreach (var pa in place.PlaceActivities)
                {
                    pa.ActivitieP = _context.Activities.FirstOrDefault(a => a.ActivitieId == pa.ActivitieId);
                }
            }
            return place;
        }
    }
}
