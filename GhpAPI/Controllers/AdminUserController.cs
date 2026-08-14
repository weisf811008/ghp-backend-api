using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Tags("系統管理-帳號")]
    [Authorize(Roles = "系統管理員")]
    public class AdminUserController : BaseController
    {
        private readonly AdminUserService _adminUserService;

        public AdminUserController(AppDbContext db, HistoryService historyService, AdminUserService adminUserService)
            : base(db, historyService)
        {
            _adminUserService = adminUserService;
        }

        // GET api/admin/users
        [HttpGet("users")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _adminUserService.GetAll();
            return Ok(result);
        }

        // GET api/admin/users/{id}
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _adminUserService.GetById(id);
            if (result == null) return NotFound(new { message = "帳號不存在" });
            return Ok(result);
        }

        // POST api/admin/users
        [HttpPost("users")]
        public async Task<IActionResult> Create([FromBody] CreateAdminUserDto dto)
        {
            var (success, error, id) = await _adminUserService.Create(dto, GetUsername(), GetName());
            if (!success) return Conflict(new { message = error });
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT api/admin/users/{id}
        [HttpPut("users/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminUserDto dto)
        {
            var (success, error) = await _adminUserService.Update(id, dto, GetUsername(), GetName());
            if (!success) return NotFound(new { message = error });
            return NoContent();
        }

        // DELETE api/admin/users/{id}
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _adminUserService.Delete(id, GetUsername(), GetName());
            if (!success) return NotFound(new { message = error });
            return NoContent();
        }

        // PATCH api/admin/users/{id}/password
        [HttpPatch("users/{id}/password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
        {
            var (success, error) = await _adminUserService.ResetPassword(id, dto, GetUsername(), GetName());
            if (!success) return NotFound(new { message = error });
            return NoContent();
        }
    }
}