using AuthApp.Dto;
using AuthApp.Interfaces;
using AuthApp.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthApp.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public UserController(IUserRepository userRepository, IMapper mapper, IConfiguration config)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _config = config;
        }

        [HttpPost("register")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto user)
        {
            if (user == null)
                return BadRequest(ModelState);

            if (await _userRepository.UserExists(user.Username))
                return BadRequest("Username already exists!");

            var userMapped = _mapper.Map<User>(user);

            var createdUser = await _userRepository.Register(userMapped, user.Password);

            if (createdUser != null)
                return StatusCode(201);

            return BadRequest("Registration Failed");

        }

        [HttpPost("login")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var user = await _userRepository.Login(userLoginDto.Username, userLoginDto.Password);
            if (user == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { tokenString });

        }

        [Authorize]
        [HttpPost("changePassword")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ChangePassword([FromQuery] int userId, ChangePasswordDto changePasswordDto)
        {
            var res = await _userRepository.ChangePassword(userId, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!res)
                return BadRequest(ModelState);

            return Ok("Password changed successfully!");
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers(string searchTerm)
        {
            var users = await _userRepository.SearchUsers(searchTerm);
            
            return Ok(users);
        }

    }
}
