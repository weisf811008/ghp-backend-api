using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/admin/schools")]
    [ApiController]
    [Tags("系統管理-學校")]
    [Authorize(Roles = "系統管理員")]
    public class SchoolsController : BaseController
    {
        public SchoolsController(AppDbContext db, HistoryService historyService)
         : base(db, historyService)
        {
        }

        // GET api/admin/schools
        [HttpGet]
        public async Task<IActionResult> GetSchools()
        {
            var schools = await _db.Schools
                .Where(s => s.DeletedAt == null)
                .Select(s => new SchoolDetailDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    City = s.City,
                    Address = s.Address,
                    Phone = s.Phone,
                    Url = s.Url,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    DeletedAt = s.DeletedAt
                })
                .ToListAsync();
            return Ok(schools);
        }

        // GET api/admin/schools/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var school = await _db.Schools
                .Where(s => s.Id == id && s.DeletedAt == null)
                .Select(s => new SchoolDetailDto
                {
                    Id = s.Id,
                    Code = s.Code,
                    Name = s.Name,
                    City = s.City,
                    Address = s.Address,
                    Phone = s.Phone,
                    Url = s.Url,
                    CreatedAt = s.CreatedAt,
                    UpdatedAt = s.UpdatedAt,
                    DeletedAt = s.DeletedAt
                })
                .FirstOrDefaultAsync();
            if (school == null)
            {
                return NotFound(new { message = "學校不存在" });
            }
            return Ok(school);
        }

        // POST api/admin/schools
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSchoolDto dto)
        {
            var exists = await _db.Schools.AnyAsync(s => s.Code == dto.Code && s.DeletedAt == null);

            if (exists)
            {
                return BadRequest(new { message = "學校編號已存在" });
            }

            var school = new School
            {
                Code = dto.Code,
                Name = dto.Name,
                City = dto.City,
                Address = dto.Address,
                Phone = dto.Phone,
                Url = dto.Url,
            };

            _db.Schools.Add(school);
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "新增學校",
                username: GetUsername(),
                name: GetName(),
                schoolId: school.Id,
                controller: nameof(SchoolsController),
                instanceKey: school.Id.ToString()
            );

            return CreatedAtAction(nameof(GetById), new { id = school.Id }, new { id = school.Id });
        }

        // PUT api/admin/schools/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSchoolDto dto)
        {
            var school = await _db.Schools.FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null);
            if (school == null)
            {
                return NotFound(new { message = "學校不存在" });
            }

            school.Name = dto.Name!;
            school.City = dto.City;
            school.Address = dto.Address;
            school.Phone = dto.Phone;
            school.Url = dto.Url;
            school.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await _historyService.Info(
               "修改學校",
               username: GetUsername(),
               name: GetName(),
               schoolId: school.Id,
               controller: nameof(SchoolsController),
               instanceKey: school.Id.ToString()
           );

            return NoContent();
        }

        // DELETE api/admin/schools/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var school = await _db.Schools.FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null);
            if (school == null)
            {
                return NotFound(new { message = "學校不存在" });
            }
            school.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await _historyService.Info(
               "刪除學校",
               username: GetUsername(),
               name: GetName(),
               schoolId: school.Id,
               controller: nameof(SchoolsController),
               instanceKey: school.Id.ToString()
           );

            return NoContent();
        }
    }
}
