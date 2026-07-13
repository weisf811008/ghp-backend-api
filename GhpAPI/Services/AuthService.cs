using GhpAPI.Data;
using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GhpAPI.Services
{
    public class AuthService
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public AuthService(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public async Task<User?> ValidateUser(string username, string password)
        {
            var user = await _db.Users
                .Include(u => u.School)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.Username == username && u.DeletedAt == null)
                .FirstOrDefaultAsync();

            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password)) return null;

            return user;
        }

        public string GenerateToken(User user)
        {
            var secret = _config["Jwt:Secret"]!;
            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"]!);

            var claims = new List<Claim>
            {
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("userName", user.Name ?? ""),
                new Claim("schoolId", user.SchoolId.ToString()),
                new Claim("needToChangePass", user.NeedToChangePass.ToString()),
            };

            // 加入角色
            foreach (var userRole in user.UserRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, userRole.Role!.Title));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}