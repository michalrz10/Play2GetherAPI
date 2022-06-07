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
using System.Net.Mail;
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
            if(!user.IsActive) return StatusCode(500, "Confirm your email");
            if (user != null)
            {
                string token = GenerateToken(user);
                return Ok(token);
            }
            return NotFound("User not found!");
        }

        [HttpGet("Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok();
        }

        [HttpPost("RegisterUser")]
        [AllowAnonymous]
        public IActionResult RegisterUser([FromBody] Login login)
        {
            if(_context.Logins.FirstOrDefault(u=>u.Email.Equals(login.Email))!=null)
            {
                return BadRequest("Email is already used!");
            }

            User user = new User()
            {
                ImageUrl = "http://87.205.116.41:5000/api/Basic/images/default.jpg",
                IsActive = false
            };
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
            string url = SendEmail(user.UserId, login.Email);
            _context.Logins.Add(userLogin);
            if (_context.SaveChanges() == 1)
            {
                return Ok(url);
            }
            
            return StatusCode(500, "Error while adding login!");
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public IActionResult Register([FromBody] RegisterUser login)
        {
            if (_context.Logins.FirstOrDefault(u => u.Email.Equals(login.Email)) != null)
            {
                return BadRequest("Email is already used!");
            }

            User user = new User()
            {
                Name = login.Name,
                SurName = login.SurName,
                Age = login.Age,
                ImageUrl = "http://87.205.116.41:5000/api/Basic/images/default.jpg",
                IsActive = false
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
            string url = SendEmail(user.UserId, login.Email);
            _context.Logins.Add(userLogin);
            if (_context.SaveChanges() == 1)
            {
                return Ok(url);
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
                Role = "Admin",
                ImageUrl = "http://87.205.116.41:5000/api/Basic/images/default.jpg",
                IsActive = false
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
            string url = SendEmail(user.UserId, login.Email);
            _context.Logins.Add(userLogin);
            if (_context.SaveChanges() == 1)
            {
                return Ok(url);
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

        [HttpGet("ConfirmEmail/{url}")]
        public IActionResult ConfirmEmail(string url)
        {
            EmailToken emailToken = _context.EmailTokens.FirstOrDefault(et=>et.Url.Equals(url));
            if (emailToken == null) return BadRequest("Wrong token");
            emailToken.UserToken = _context.Users.FirstOrDefault(u=>u.UserId==emailToken.UserId);
            emailToken.UserToken.IsActive=true;
            _context.EmailTokens.Remove(emailToken);
            _context.SaveChanges();
            return Ok("Email confirmed!");
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
                //expires: DateTime.Now.AddMonths(3),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(Login userLogin)
        {
            Login login = _context.Logins.FirstOrDefault(l => l.Email.Equals(userLogin.Email) && l.Password.Equals(userLogin.Password));
            if (login == null) return null;
            var currentUser = _context.Users.Find(login.LoginId);

            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }

        private string SendEmail(long id, string email)
        {
            MailMessage message = new MailMessage("Play2GetherReg@gmail.com", email);
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();
            char letter;
            do
            {
                str_build.Clear();
                for (int i = 0; i <16; i++)
                {
                    letter = Convert.ToChar(random.Next(25) + 65);
                    str_build.Append(letter);
                }
            } while (_context.EmailTokens.FirstOrDefault(et=>et.Url.Equals(str_build.ToString()))!=null);
            var et = new EmailToken()
            {
                UserId = id,
                Url = str_build.ToString()
            };
            _context.EmailTokens.Add(et);
            _context.SaveChanges();
            /*message.Subject = "Play2Gether - Confirm Email";
            message.Body = "http://127.0.0.1:5000/ConfirmEmail/" + str_build.ToString();
            message.BodyEncoding = Encoding.UTF8;
            message.IsBodyHtml = true;
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); //Gmail smtp    
            System.Net.NetworkCredential basicCredential1 = new
            System.Net.NetworkCredential("Play2GetherReg@gmail.com", "Play2GetherReg123");
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = basicCredential1;
            try
            {
                client.Send(message);
            }

            catch (Exception ex)
            {
                throw ex;
            }*/
            return "http://87.205.116.41:5000/api/Login/ConfirmEmail/" + str_build.ToString();
        }
    }
}
