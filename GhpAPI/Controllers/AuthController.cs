using GhpAPI.DTOs;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GhpAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Tags("身分驗證")]

    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        //POST api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _authService.ValidateUser(dto.Username, dto.Password);

            if (user == null)
            {
                return Unauthorized(new { message = "帳號或密碼錯誤" });
            }

            var token = _authService.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Name = user.Name,
                    Phone = user.Phone,
                    NeedToChangePass = user.NeedToChangePass,
                    Reserved = user.Reserved,
                    SchoolId = user.SchoolId,
                    School = user.School == null ? null : new SchoolDto
                    {
                        Code = user.School.Code,
                        Name = user.School.Name,
                    },
                    Roles = user.UserRoles.Select(ur => new UserRoleDto
                    {
                        Role = ur.Role!.Title,
                        Reserved = ur.Reserved,
                    }).ToList()
                }
            });
        }

        //POST api/auth/me
        [HttpGet("me")]
        [Authorize]

        public IActionResult Me()
        {
            var id = User.FindFirst("id")?.Value;
            var username = User.FindFirst("username")?.Value;
            var name = User.FindFirst("userName")?.Value;
            var schoolId = User.FindFirst("schoolId")?.Value;
            var needToChangePass = User.FindFirst("needToChangePass")?.Value;
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                id = int.Parse(id!),
                username,
                name,
                schoolId = int.Parse(schoolId!),
                needToChangePass = bool.Parse(needToChangePass!),
                roles
            }
            );
        }
    }
}
