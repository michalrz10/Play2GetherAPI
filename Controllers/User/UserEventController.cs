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
    public class UserEventController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public UserEventController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpGet("GetEvents")]
        public ActionResult<IEnumerable<Event>> GetEvents()
        {
            var list1 = _context.Events.AsNoTracking().ToList();
            foreach(var e in list1)
            {
                e.Organizer = _context.Users.FirstOrDefault(u => u.UserId == e.UserId);
                e.ActivitieEvent = _context.Activities.FirstOrDefault(a => a.ActivitieId == e.ActivitieId);
                e.Spot = _context.Places.FirstOrDefault(p=> p.PlaceId == e.PlaceId);
                e.UsersEvents = _context.UserEvents.Where(u => u.EventId == e.EventId).AsNoTracking().ToList();
                foreach(var u in e.UsersEvents)
                {
                    u.UserE=_context.Users.FirstOrDefault(uu => uu.UserId == u.UserId);
                    if (u.UserE != null)
                    {
                        Premium premium = _context.Premiums.Where(p => p.UserId.Equals(u.UserE.UserId)).OrderBy(p => p.StartDate).FirstOrDefault();
                        if (premium != null)
                        {
                            if (DateTime.Now < premium.StartDate.AddDays(premium.Days))
                            {
                                u.UserE.isPremium = true;
                            }
                            u.UserE.isPremium = false;
                        }
                        u.UserE.isPremium = false;
                        if(u.UserE.Role.Equals("Admin")) u.UserE.isPremium = true;
                    }
                }
                /*e.Spot.PlaceActivities = _context.PlaceActivities.Where(pa => pa.PlaceId == e.Spot.PlaceId).AsNoTracking().ToList();
                foreach (var pa in e.Spot.PlaceActivities)
                {
                    pa.ActivitieP = _context.Activities.FirstOrDefault(a => a.ActivitieId == pa.ActivitieId);
                }*/
            }
            return list1;
        }

        [HttpGet("GetEvent/{id}")]
        public ActionResult<Event> GetEvents(long id)
        {
            var even = _context.Events.AsNoTracking().FirstOrDefault(e => e.EventId == id);
            if(even != null)
            {
                even.Organizer = _context.Users.FirstOrDefault(u => u.UserId == even.UserId);
                even.ActivitieEvent = _context.Activities.FirstOrDefault(a => a.ActivitieId == even.ActivitieId);
                even.Spot = _context.Places.FirstOrDefault(p => p.PlaceId == even.PlaceId);
                even.UsersEvents = _context.UserEvents.Where(u => u.EventId == even.EventId).AsNoTracking().ToList();
                foreach (var u in even.UsersEvents)
                {
                    u.UserE = _context.Users.FirstOrDefault(uu => uu.UserId == u.UserId);
                    if (u.UserE != null)
                    {
                        Premium premium = _context.Premiums.Where(p => p.UserId.Equals(u.UserE.UserId)).OrderBy(p => p.StartDate).FirstOrDefault();
                        if (premium != null)
                        {
                            if (DateTime.Now < premium.StartDate.AddDays(premium.Days))
                            {
                                u.UserE.isPremium = true;
                            }
                            u.UserE.isPremium = false;
                        }
                        u.UserE.isPremium = false;
                        if (u.UserE.Role.Equals("Admin")) u.UserE.isPremium = true;
                    }
                }
                /*e.Spot.PlaceActivities = _context.PlaceActivities.Where(pa => pa.PlaceId == e.Spot.PlaceId).AsNoTracking().ToList();
                foreach (var pa in e.Spot.PlaceActivities)
                {
                    pa.ActivitieP = _context.Activities.FirstOrDefault(a => a.ActivitieId == pa.ActivitieId);
                }*/
            }
            return even;
        }

        [HttpPost("AddEvent")]
        public IActionResult AddEvent([FromBody] Event newevent)
        {
            if (_context.Activities.FirstOrDefault(a => a.ActivitieId==newevent.ActivitieId) == null)
            {
                return BadRequest("No activitie with that id");
            }
            if (_context.Places.FirstOrDefault(a => a.PlaceId == newevent.PlaceId) == null)
            {
                return BadRequest("No place with that id");
            }
            newevent.UserId = long.Parse((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            if (_context.Users.FirstOrDefault(a => a.UserId == newevent.UserId) == null)
            {
                return BadRequest("No user with that id");
            }
            _context.Events.Add(newevent);
            if (_context.SaveChanges() != 1) return StatusCode(500, "Error while adding event!");
            return Ok();
        }

        [HttpPost("JoinEvent/{eventid}")]
        public IActionResult JoinEvent(long eventid)
        {
            var id = long.Parse((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var tempEvent = _context.Events.FirstOrDefault(e => e.EventId == eventid);
            if (tempEvent == null) return BadRequest("No event with that id");
            if (_context.UserEvents.Where(ue => ue.UserId == id && ue.EventId == tempEvent.EventId).Count() != 0) return BadRequest("User already joined that event");
            if (tempEvent.UserId == id) return BadRequest("User Organazing this event");
            if (tempEvent.Vacancies <= 0) return BadRequest("Vacancies error");
            var ue = new UserEvent()
            {
                UserId = id,
                EventId = tempEvent.EventId
            };
            _context.UserEvents.Add(ue);
            tempEvent.Vacancies -= 1;
            if (_context.SaveChanges() != 2) return StatusCode(500, "Error while joining event!");
            return Ok();
        }

        [HttpPost("LeaveEvent/{eventid}")]
        public IActionResult LeaveEvent(long eventid)
        {
            var id = long.Parse((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var tempEvent = _context.Events.FirstOrDefault(e => e.EventId == eventid);
            if (tempEvent == null) return BadRequest("No event with that id");
            if (_context.UserEvents.Where(ue => ue.UserId == id && ue.EventId == tempEvent.EventId).Count() != 1) return BadRequest("User not joined event");
            var ue = _context.UserEvents.FirstOrDefault(ue => ue.UserId == id && ue.EventId == eventid);
            _context.UserEvents.Remove(ue);
            tempEvent.Vacancies += 1;
            if (_context.SaveChanges() != 2) return StatusCode(500, "Error while joining event!");
            return Ok();
        }
    }
}
