using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Play2GetherAPI.DAL;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public AdminPanelController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }


    }
}
