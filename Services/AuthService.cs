using LeaderboardAPI.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LeaderboardAPI.DTOs;
using LeaderboardAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace LeaderboardAPI.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<ServiceResponse<string>> RegisterAsync(UserRegisterDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                return new ServiceResponse<string> { Success = false, Message = "User already exists." };

            var user = new User
            {
                Username = request.Username,
                //BCrypt şifre hash'leme işlemi için kullanılan bir kütüphanedir
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                RegistrationDate = DateTime.UtcNow,
                PlayerLevel = 1,
                TrophyCount = 0
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new ServiceResponse<string> { Success = true, Message = "User registered." };
        }

        public async Task<ServiceResponse<string>> LoginAsync(UserLoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return new ServiceResponse<string> { Success = false, Message = "Invalid credentials." };

            var token = GenerateJwtToken(user);
            return new ServiceResponse<string> { Success = true, Data = token };
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
