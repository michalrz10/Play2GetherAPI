using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Play2GetherAPI.ControllerModels;
using Play2GetherAPI.DAL;
using Play2GetherAPI.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Play2GetherAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly ApiDbContext _context;

        public LoginController(IConfiguration config, ApiDbContext context)
        {
            _config = config;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] Login userLogin)
        {
            User user = Authenticate(userLogin);

            if (user != null)
            {
                string token = GenerateToken(user);
                return Ok(token);
            }
            return NotFound("User not found!");
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] Login login)
        {
            if(_context.Logins.FirstOrDefault(u=>u.Email.Equals(login.Email))!=null)
            {
                return BadRequest("Email is already used!");
            }
            
            User user = new User();
            _context.Users.Add(user);
            if (_context.SaveChanges() != 1)
            {
                return StatusCode(500,"Error while adding user!");
            }
            Login userLogin = new Login
            {
                LoginId = user.UserId,
                Email = login.Email,
                Password = login.Password
            };
            _context.Logins.Add(userLogin);
            if (_context.SaveChanges() == 1)
            {
                return Ok("User has been created!");
            }

            return StatusCode(500, "Error while adding login!");
        }
        [HttpPost("RegisterAdmin")]
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterAdmin([FromBody] Login login)
        {
            if (_context.Logins.FirstOrDefault(u => u.Email.Equals(login.Email)) != null)
            {
                return BadRequest("Email is already used!");
            }

            User user = new User
            {
                Role = "Admin"
            };
            _context.Users.Add(user);
            if (_context.SaveChanges() != 1)
            {
                return StatusCode(500, "Error while adding user!");
            }
            Login userLogin = new Login
            {
                LoginId = user.UserId,
                Email = login.Email,
                Password = login.Password
            };
            _context.Logins.Add(userLogin);
            if (_context.SaveChanges() == 1)
            {
                return Ok("Admin has been created!");
            }

            return StatusCode(500, "Error while adding login!");
        }

        [HttpPost("ChangePassword")]
        [Authorize]
        public IActionResult ChangePassword([FromBody] ChPassword chPassword)
        {
            Login login = _context.Logins.Find(long.Parse((HttpContext.User.Identity as ClaimsIdentity).Claims.FirstOrDefault(c=>c.Type == ClaimTypes.NameIdentifier)?.Value));
            if(login != null)
            {
                if(login.Password.Equals(chPassword.currentPassword))
                {
                    login.Password = chPassword.newPassword;
                    _context.SaveChanges();
                    return Ok("Password has been changed!");
                }
                return BadRequest("Wrong Password!");
            }
            return NotFound("Theres no user with that id!");
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(Login userLogin)
        {
            var currentUser = _context.Users.Find(_context.Logins.FirstOrDefault(l=>l.Email.Equals(userLogin.Email) && l.Password.Equals(userLogin.Password)).LoginId);

            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }
    }
}
