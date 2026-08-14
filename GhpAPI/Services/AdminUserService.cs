using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Services
{
    public class AdminUserService
    {
        private readonly AppDbContext _db;
        private readonly HistoryService _historyService;

        public AdminUserService(AppDbContext db, HistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        public async Task<List<AdminUserDto>> GetAll()
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
                }
            ).ToListAsync();

            var userIds = users.Select(u => u.Id).ToList();
            var userRoles = await (
                from ur in _db.UserRoles
                join r in _db.Roles on ur.RoleId equals r.Id
                where userIds.Contains(ur.UserId)
                select new { ur.UserId, RoleTitle = r.Title, ur.Reserved }
            ).ToListAsync();

            return users.Select(u => new AdminUserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Name = u.Name,
                Phone = u.Phone,
                NeedToChangePass = u.NeedToChangePass,
                Reserved = u.Reserved,
                SchoolId = u.SchoolId,
                School = new SchoolDto { Code = u.SchoolCode, Name = u.SchoolName },
                Roles = userRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Select(ur => new UserRoleDto { Role = ur.RoleTitle, Reserved = ur.Reserved })
                    .ToList(),
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt,
                DeletedAt = u.DeletedAt,
            }).ToList();
        }

        public async Task<AdminUserDto?> GetById(int id)
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

            if (user == null) return null;

            var userRoles = await (
                from ur in _db.UserRoles
                join r in _db.Roles on ur.RoleId equals r.Id
                where ur.UserId == id
                select new { RoleTitle = r.Title, ur.Reserved }
            ).ToListAsync();

            return new AdminUserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Name = user.Name,
                Phone = user.Phone,
                NeedToChangePass = user.NeedToChangePass,
                Reserved = user.Reserved,
                SchoolId = user.SchoolId,
                School = new SchoolDto { Code = user.SchoolCode, Name = user.SchoolName },
                Roles = userRoles.Select(ur => new UserRoleDto { Role = ur.RoleTitle, Reserved = ur.Reserved }).ToList(),
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                DeletedAt = user.DeletedAt,
            };
        }

        public async Task<(bool success, string? error, int? id)> Create(CreateAdminUserDto dto, string username, string name)
        {
            var exist = await _db.Users.AnyAsync(u => u.Username == dto.Username && u.DeletedAt == null);
            if (exist) return (false, "帳號已存在", null);

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
                username,
                name,
                user.SchoolId,
                controller: nameof(AdminUserService),
                instanceKey: user.Id.ToString()
            );

            if (dto.Roles != null && dto.Roles.Count > 0)
            {
                var roles = await _db.Roles
                    .Where(r => dto.Roles.Contains(r.Title) && r.DeletedAt == null)
                    .ToListAsync();

                foreach (var role in roles)
                {
                    _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
                }
                await _db.SaveChangesAsync();
            }

            return (true, null, user.Id);
        }

        public async Task<(bool success, string? error)> Update(int id, UpdateAdminUserDto dto, string username, string name)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
            if (user == null) return (false, "帳號不存在");

            user.Email = dto.Email;
            user.Name = dto.Name;
            user.Phone = dto.Phone;
            user.SchoolId = dto.SchoolId;
            user.UpdatedAt = DateTime.UtcNow;

            var existingRoles = await _db.UserRoles.Where(ur => ur.UserId == id).ToListAsync();
            _db.UserRoles.RemoveRange(existingRoles);

            if (dto.Roles != null && dto.Roles.Count > 0)
            {
                var roles = await _db.Roles
                    .Where(r => dto.Roles.Contains(r.Title) && r.DeletedAt == null)
                    .ToListAsync();

                foreach (var role in roles)
                {
                    _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = role.Id });
                }
            }

            await _db.SaveChangesAsync();

            await _historyService.Info(
                "修改帳號",
                username,
                name,
                user.SchoolId,
                controller: nameof(AdminUserService),
                instanceKey: user.Id.ToString()
            );

            return (true, null);
        }

        public async Task<(bool success, string? error)> Delete(int id, string username, string name)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
            if (user == null) return (false, "帳號不存在");

            user.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _historyService.Info(
                "刪除帳號",
                username,
                name,
                user.SchoolId,
                controller: nameof(AdminUserService),
                instanceKey: user.Id.ToString()
            );

            return (true, null);
        }

        public async Task<(bool success, string? error)> ResetPassword(int id, ResetPasswordDto dto, string username, string name)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.DeletedAt == null);
            if (user == null) return (false, "帳號不存在");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.NeedToChangePass = false;
            await _db.SaveChangesAsync();

            await _historyService.Info(
                "重設帳號密碼",
                username,
                name,
                user.SchoolId,
                controller: nameof(AdminUserService),
                instanceKey: user.Id.ToString()
            );

            return (true, null);
        }
    }
}