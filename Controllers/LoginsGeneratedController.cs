using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginsGeneratedController : ControllerBase
    {
        private readonly ApiDbContext _context;

        public LoginsGeneratedController(ApiDbContext context)
        {
            _context = context;
        }

        // GET: api/LoginsGenerated
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Login>>> GetLogins()
        {
            return await _context.Logins.ToListAsync();
        }

        // GET: api/LoginsGenerated/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Login>> GetLogins(long id)
        {
            var logins = await _context.Logins.FindAsync(id);

            if (logins == null)
            {
                return NotFound();
            }

            return logins;
        }

        // PUT: api/LoginsGenerated/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogins(long id, Login logins)
        {
            if (id != logins.LoginId)
            {
                return BadRequest();
            }

            _context.Entry(logins).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoginsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/LoginsGenerated
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Login>> PostLogins(Login logins)
        {
            _context.Logins.Add(logins);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLogins", new { id = logins.LoginId }, logins);
        }

        // DELETE: api/LoginsGenerated/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Login>> DeleteLogins(long id)
        {
            var logins = await _context.Logins.FindAsync(id);
            if (logins == null)
            {
                return NotFound();
            }

            _context.Logins.Remove(logins);
            await _context.SaveChangesAsync();

            return logins;
        }

        private bool LoginsExists(long id)
        {
            return _context.Logins.Any(e => e.LoginId == id);
        }
    }
}
