using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using bin.Data;
using bin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace bin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AppSettings _appSettings;

        public UserController(ApplicationDbContext context, IOptions<AppSettings> appsettings)
        {
            _context = context;
            _appSettings = appsettings.Value;

            if (!_context.Users.Any())
            {
                _context.Users.Add(new User { Username = "Test", Password = "Test" });
                _context.Users.Add(new User { Username = "Test2", Password = "Test2" });
                _context.SaveChanges();
            }
        }

          
        [HttpGet]
        public IActionResult Get()
        {
            List<User> users = _context.Users.ToList();
            return Ok(users);
        }

        [HttpPost("register")]
        public IActionResult Register(AuthenticationModel userDetail)
        {
            var checkExist = _context.Users.FirstOrDefault(x => x.Username.ToLower() == userDetail.Username.ToLower());
            if (checkExist != null)
            {
                return BadRequest("Username already exist");
            }

            var user = new User
            {
                Username = userDetail.Username,
                Password = userDetail.Password
            };
                
             _context.Add(user);
            _context.SaveChanges();
            return Ok(user);
        }

        [HttpPost("authenticate")]
        public IActionResult Get(AuthenticationModel user)
        {
            var Loguser = Authenticate(user.Username, user.Password);
            return Ok(Loguser);
        }

        private User Authenticate(string username, string password)
        {
            var getUsers = _context.Users;
                
            var user = getUsers.FirstOrDefault(x => x.Username.ToLower() == username.ToLower() && x.Password == password);

            //user not found
            if (user == null)
            {
                return null;
            }

            //if user was found generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials
                                (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);
            user.Password = "";
            return user;
        }
    }
}
