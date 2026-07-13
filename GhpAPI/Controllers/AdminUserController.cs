using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Tags("系統管理-帳號")]
    [Authorize(Roles = "系統管理員")]
    public class AdminUserController : BaseController
    {
        public AdminUserController(AppDbContext db, HistoryService historyService)
        : base(db, historyService)
        {
        }

        // GET api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAll()
        {
            var users = await (
                from u in _db.Users
                join s in _db.Schools on u.SchoolId equals s.Id
                where u.DeletedAt == null
                select new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.Name,
                    u.Phone,
                    u.NeedToChangePass,
                    u.Reserved,
                    u.SchoolId,
                    SchoolCode = s.Code,
                    SchoolName = s.Name,
                    u.CreatedAt,
                    u.UpdatedAt,
                    u.DeletedAt
                }).ToListAsync();

            var userIds = users.Select(u => u.Id).ToList();
            var userRoles = await (
                from ur in _db.UserRoles
                join r in _db.Roles on ur.RoleId equals r.Id
                where userIds.Contains(ur.UserId)
                select new
                {
                    ur.UserId,
                    RoleTitle = r.Title,
                    ur.Reserved
                }).ToListAsync();

            var result = users.Select(u => new AdminUserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Name = u.Name,
                Phone = u.Phone,
                NeedToChangePass = u.NeedToChangePass,
                Reserved = u.Reserved,
                SchoolId = u.SchoolId,
                School = new SchoolDto
                {
                    Code = u.SchoolCode,
                    Name = u.SchoolName,
                },
                Roles = userRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Select(ur => new UserRoleDto
                    {
                        Role = ur.RoleTitle,
                        Reserved = ur.Reserved,
                    }).ToList(),
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                DeletedAt = u.DeletedAt,
            }).ToList();

            return Ok(result);
        }

        // GET api/admin/users/{id}
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await (
                    from u in _db.Users
                    join s in _db.Schools on u.SchoolId equals s.Id
                    where u.Id == id && u.DeletedAt == null
                    select new
                    {
                        u.Id,
                        u.Username,
                        u.Email,
                        u.Name,
                        u.Phone,
                        u.NeedToChangePass,
                        u.Reserved,
                        u.SchoolId,
                        SchoolCode = s.Code,
                        SchoolName = s.Name,
                        u.CreatedAt,
                        u.UpdatedAt,
                        u.DeletedAt
                    }
                ).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound(new { message = "帳號不存在" });
            }

            var userRoles = await (
                    from ur in _db.UserRoles
                    join r in _db.Roles on ur.RoleId equals r.Id
                    where ur.UserId == id
                    select new
                    {
                        RoleTitle = r.Title,
                        ur.Reserved
                    }
                ).ToListAsync();

            var result = new AdminUserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone,
                NeedToChangePass = user.NeedToChangePass,
                Reserved = user.Reserved,
                SchoolId = user.SchoolId,
                School = new SchoolDto
                {
                    Code = user.SchoolCode,
                    Name = user.SchoolName,
                },
                Roles = userRoles.Select(ur => new UserRoleDto
                {
                    Role = ur.RoleTitle,
                    Reserved = ur.Reserved
                }).ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                DeletedAt = user.DeletedAt,
            };
            return Ok(result);
        }

        //POST api/admin/users
        [HttpPost("users")]
        public async Task<IActionResult> Create([FromBody] CreateAdminUserDto dto)
        {
            var existe = await _db.Users.AnyAsync(u => u.Username == dto.Username && u.DeletedAt == null);

            if (existe)
            {
                return BadRequest(new { message = "帳號已存在" });
            }

            var user = new User
            {
                Username = dto.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Email = dto.Email,
                Name = dto.Name,
                Phone = dto.Phone,
                SchoolId = dto.SchoolId,
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            await _historyService.Info(
               "新增帳號",
               username: GetUsername(),
               name: GetName(),
               schoolId: user.SchoolId,
               controller: nameof(AdminUserController),
               instanceKey: user.Id.ToString()
           );

            if (dto.Roles != null && dto.Roles.Count > 0)
            {
                var roles = await _db.Roles
                    .Where(r => dto.Roles.Contains(r.Title) && r.DeletedAt == null)
                    .ToListAsync();

                foreach (var role in roles)
                {
                    _db.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = role.Id,
                    });
                }
            }

            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { id = user.Id });
        }


        // PUT api/admin/users/{id}
        [HttpPut("users/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminUserDto dto)
        {
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);

            if (user == null)
            {
                return NotFound(new { message = "帳號不存在" });
            }

            user.Email = dto.Email;
            user.Name = dto.Name;
            user.Phone = dto.Phone;
            user.SchoolId = dto.SchoolId;
            user.UpdatedAt = DateTime.UtcNow;

            // 先查出舊的角色，再刪除
            var existingRoles = await _db.UserRoles
                .Where(ur => ur.UserId == id)
                .ToListAsync();

            _db.UserRoles.RemoveRange(existingRoles);

            // 新增新的角色
            if (dto.Roles != null && dto.Roles.Count > 0)
            {
                var roles = await _db.Roles
                    .Where(r => dto.Roles.Contains(r.Title) && r.DeletedAt == null)
                    .ToListAsync();

                foreach (var role in roles)
                {
                    _db.UserRoles.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = role.Id,
                    });
                }
            }

            await _db.SaveChangesAsync();
            await _historyService.Info(
               "修改帳號",
               username: GetUsername(),
               name: GetName(),
               schoolId: user.SchoolId,
               controller: nameof(AdminUserController),
               instanceKey: user.Id.ToString()
           );

            return NoContent();
        }

        // DELETE api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
            if (user == null)
            {
                return NotFound(new { message = "帳號不存在" });
            }
            user.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await _historyService.Info(
               "刪除帳號",
               username: GetUsername(),
               name: GetName(),
               schoolId: user.SchoolId,
               controller: nameof(AdminUserController),
               instanceKey: user.Id.ToString()
           );
            return NoContent();
        }

        //PATCH api/admin/users/{id}/password
        [HttpPatch("users/{id}/password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
            if (user == null)
            {
                return NotFound(new { message = "帳號不存在" });
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.NeedToChangePass = false;

            await _db.SaveChangesAsync();
            await _historyService.Info(
               "重設帳號密碼",
               username: GetUsername(),
               name: GetName(),
               schoolId: user.SchoolId,
               controller: nameof(AdminUserController),
               instanceKey: user.Id.ToString()
           );

            return NoContent();
        }
    }
}
