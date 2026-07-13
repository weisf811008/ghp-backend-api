using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/regulations")]
    [ApiController]
    [Tags("條文管理")]
    [Authorize]
    public class RegulationsController : BaseController
    {
        public RegulationsController(AppDbContext db, HistoryService historyService)
         : base(db, historyService)
        {
        }

        //GET api/regulations
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetAll()
        {
            var schoolId = GetSchoolId();
            var regulations = await _db.Regulations
                .Where(r => r.SchoolId == schoolId && r.DeletedAt == null)
                .Select(r => new RegulationDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Class = r.Class,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    DeletedAt = r.DeletedAt
                }).ToListAsync();
            return Ok(regulations);
        }

        //GET api/regulations/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]
        public async Task<IActionResult> GetById(int id)
        {
            var schoolId = GetSchoolId();
            var regulation = await _db.Regulations
                .Where(r => r.Id == id && r.SchoolId == schoolId && r.DeletedAt == null)
                .Select(r => new RegulationDto
                {
                    Id = r.Id,
                    Code = r.Code,
                    Class = r.Class,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                    DeletedAt = r.DeletedAt
                }).FirstOrDefaultAsync();
            if (regulation == null)
            {
                return NotFound(new { message = "條文不存在" });
            }
            return Ok(regulation);
        }


        //POST api/regulations
        [HttpPost]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Create([FromBody] SaveRegulationDto dto)
        {
            var schoolId = GetSchoolId();
            var exist = await _db.Regulations.AnyAsync(r => r.Code == dto.Code && r.SchoolId == schoolId && r.DeletedAt == null);
            if (exist)
            {
                return BadRequest(new { message = "條文已存在" });
            }
            var regulation = new Regulation
            {
                Code = dto.Code,
                Class = dto.Class,
                Description = dto.Description,
                SchoolId = schoolId,
            };
            _db.Regulations.Add(regulation);
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "新增條文",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(RegulationsController),
                instanceKey: regulation.Id.ToString()
            );

            return CreatedAtAction(nameof(GetById), new { id = regulation.Id }, new { id = regulation.Id });
        }

        //PUT api/regulations/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Update(int id, [FromBody] SaveRegulationDto dto)
        {
            var schoolId = GetSchoolId();

            var regulation = await _db.Regulations.FirstOrDefaultAsync(r => r.Id == id && r.SchoolId == schoolId && r.DeletedAt == null);

            if (regulation == null)
            {
                return NotFound(new { message = "條文不存在" });
            }

            regulation.Code = dto.Code;
            regulation.Class = dto.Class;
            regulation.Description = dto.Description;
            regulation.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await _historyService.Info(
                "修改條文",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(RegulationsController),
                instanceKey: regulation.Id.ToString()
            );
            return NoContent();
        }

        //DELETE api/regulations/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Delete(int id)
        { 
            var schoolId = GetSchoolId();
            var regulation = await _db.Regulations.FirstOrDefaultAsync(r => r.Id == id && r.SchoolId == schoolId && r.DeletedAt == null);
            if (regulation == null)
            { 
                return NotFound(new { message = "條文不存在" });
            }

            regulation.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "刪除條文",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(RegulationsController),
                instanceKey: regulation.Id.ToString()
            );
            return NoContent();
        }
    }
}

