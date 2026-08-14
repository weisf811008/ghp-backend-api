using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    [Tags("帳號管理")]
    [Authorize(Roles = "學校管理員")]
    public class UserController : BaseController
    {
        private readonly UserService _userService;

        public UserController(AppDbContext db, HistoryService historyService, UserService userService)
            : base(db, historyService)
        {
            _userService = userService;
        }

        // GET api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _userService.GetAll(GetSchoolId());
            return Ok(result);
        }

        // GET api/users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _userService.GetById(id, GetSchoolId());
            if (result == null) return NotFound(new { message = "帳號不存在" });
            return Ok(result);
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAdminUserDto dto)
        {
            var (success, error, id) = await _userService.Create(dto, GetSchoolId(), GetUsername(), GetName());
            if (!success) return Conflict(new { message = error });
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT api/users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdminUserDto dto)
        {
            var (success, error) = await _userService.Update(id, dto, GetSchoolId(), GetUsername(), GetName());
            if (!success) return NotFound(new { message = error });
            return NoContent();
        }

        // DELETE api/users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _userService.Delete(id, GetSchoolId(), GetUsername(), GetName());
            if (!success) return NotFound(new { message = error });
            return NoContent();
        }

        // PATCH api/users/{id}/password
        [HttpPatch("{id}/password")]
        public async Task<IActionResult> ResetPassword(int id, [FromBody] ResetPasswordDto dto)
        {
            var (success, error) = await _userService.ResetPassword(id, GetSchoolId(), dto, GetUsername(), GetName());
            if (!success) return NotFound(new { message = error });
            return NoContent();
        }
    }
}