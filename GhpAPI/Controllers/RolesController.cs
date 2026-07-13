using GhpAPI.Data;
using GhpAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/roles")]
    [ApiController]
    [Tags("系統管理-權限")]
    [Authorize(Roles = "系統管理員")]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public RolesController(AppDbContext db)
        {
            _db = db;
        }

        //GET api/roles
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _db.Roles
               .Where(r => r.DeletedAt == null)
               .Select(r => new RoleDto
               {
                   Id = r.Id,
                   Title = r.Title,
                   Reserved = r.Reserved,
                   CreatedAt = r.CreatedAt,
                   UpdatedAt = r.UpdatedAt,
               }).ToListAsync();
            return Ok(roles);
        }


        //GET api/roles/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var role = await _db.Roles
                .Where(r => r.Id == id && r.DeletedAt == null)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Title = r.Title,
                    Reserved = r.Reserved,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                }).FirstOrDefaultAsync();
            if (role == null)
            {
                return NotFound();
            }
            return Ok(role);
        }
    }
}
