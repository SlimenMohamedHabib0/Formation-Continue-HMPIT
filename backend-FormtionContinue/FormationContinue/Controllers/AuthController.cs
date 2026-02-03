using FormationContinue.Data;
using FormationContinue.Dtos.Auth;
using FormationContinue.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FormationContinue.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto request)
        {
            request.Email = request.Email.Trim();

            var emailExists = await _context.Users.CountAsync(u => u.Email.ToLower() == request.Email.ToLower()) > 0;
            if (emailExists)
                return BadRequest("Email Already Used");

            var serviceExists = await _context.Services.CountAsync(s => s.Id == request.ServiceId) > 0;
            if (!serviceExists)
                return BadRequest("Service introuvable.");

            var statutExists = await _context.Statuts.CountAsync(s => s.Id == request.StatutId) > 0;
            if (!statutExists)
                return BadRequest("Statut introuvable.");

            var newUser = new User
            {
                FullName = request.FullName.Trim(),
                Email = request.Email,
                Role = "USER",
                ServiceId = request.ServiceId,
                StatutId = request.StatutId
            };

            newUser.PasswordHash = new PasswordHasher<User>()
                .HashPassword(newUser, request.Password);

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok("Registered successfully.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto request)
        {
            request.Email = request.Email.Trim();

            var user = await _context.Users
                .Include(u => u.Service)
                .Include(u => u.Statut)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());

            if (user == null)
                return Unauthorized("Invalid email or password");

            if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
                == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid email or password");

            var token = CreateToken(user);

            return Ok(new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                ServiceId = user.ServiceId,
                ServiceLibelle = user.Service.Libelle,
                StatutId = user.StatutId,
                StatutLibelle = user.Statut.Libelle,
                Token = token
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<AuthResponseDto>> Me()
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(id, out var userId))
                return Unauthorized();

            var user = await _context.Users
                .Include(u => u.Service)
                .Include(u => u.Statut)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return Unauthorized();

            return Ok(new AuthResponseDto
            {
                UserId = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                ServiceId = user.ServiceId,
                ServiceLibelle = user.Service.Libelle,
                StatutId = user.StatutId,
                StatutLibelle = user.Statut.Libelle,
                Token = string.Empty
            });
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration.GetValue<string>("AppSettings:Issuer"),
                audience: _configuration.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
