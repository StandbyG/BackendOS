using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OSControlSystem.Data;
using OSControlSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using OSControlSystem.DTOs;

namespace OSControlSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto loginDto)
        {
            var user = _context.Usuarios
            .FirstOrDefault(u => 
            u.Email.ToLower() == loginDto.Email.Trim().ToLower() &&
            u.PasswordHash == loginDto.Password);

            if (user == null)
                return Unauthorized("Credenciales inválidas");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Rol)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new
            {
                Token = tokenHandler.WriteToken(token)
            });
        }


        [Authorize(Roles = "Administrador")]
        [HttpPost("register")]
        public IActionResult Register([FromBody] UsuarioRegistroDto dto)
        {
            if (_context.Usuarios.Any(u => u.Email == dto.Email))
                return BadRequest("Ya existe un usuario con ese email.");

            if (string.IsNullOrWhiteSpace(dto.Nombre) ||
                string.IsNullOrWhiteSpace(dto.Email) ||
                string.IsNullOrWhiteSpace(dto.Password) ||
                string.IsNullOrWhiteSpace(dto.Rol))
            {
                return BadRequest("Todos los campos son obligatorios.");
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Email = dto.Email,
                Rol = dto.Rol,
                PasswordHash = dto.Password // en producción: encripta esto
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return Ok("Usuario registrado correctamente.");
        }

    }

    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
