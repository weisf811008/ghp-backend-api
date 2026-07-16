using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/visitingForms")]
    [ApiController]
    [Tags("訪視表管理")]
   
    public class VisitingFormController : BaseController
    {
        public VisitingFormController(AppDbContext db, HistoryService historyService)
        : base(db, historyService)
        {
        }

        //GET api/visitingForms
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetAll()
        {
            var schoolId = GetSchoolId();

            var visitingForms = await _db.VisitingForms
                .Where(v => v.SchoolId == schoolId && v.DeletedAt == null)
                .Select(v => new VisitingFormDto
                {
                    Id = v.Id,
                    Code = v.Code,
                    Class = v.Class,
                    Description = v.Description,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    DeletedAt = v.DeletedAt,
                }).ToListAsync();

            return Ok(visitingForms);
        }

        //GET api/visitingForms/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetById(int id)
        {
            var schoolId = GetSchoolId();

            var visitingForm = await _db.VisitingForms
                .Where(v => v.Id == id && v.SchoolId == schoolId && v.DeletedAt == null)
                .Select(v => new VisitingFormDto
                {
                    Id = v.Id,
                    Code = v.Code,
                    Class = v.Class,
                    Description= v.Description,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    DeletedAt = v.DeletedAt,
                }).FirstOrDefaultAsync();

            if (visitingForm == null)
            {
                return NotFound(new { message = "訪視表不存在" });
            }

            return Ok(visitingForm);
        }

        //POST api/visitingForms
        [HttpPost]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Create([FromBody] SaveVisitingFormDto dto)
        {
            var schoolId = GetSchoolId();

            var exist = await _db.VisitingForms.AnyAsync(v => v.Code == dto.Code && v.SchoolId == schoolId && v.DeletedAt == null);

            if (exist)
            {
                return Conflict(new { message = "訪視表已存在" });
            }

            var visitingForm = new VisitingForm
            {
                Code = dto.Code,
                Class = dto.Class,
                Description = dto.Description,
                SchoolId = schoolId,
            };
            _db.VisitingForms.Add(visitingForm);
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "新增訪視表",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(VisitingFormController),
                instanceKey: visitingForm.Id.ToString()
            );
            return CreatedAtAction(nameof(GetById), new { id = visitingForm.Id }, new { id = visitingForm.Id });

        }

        //PUT api/visitingForms/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Update(int id, [FromBody] SaveVisitingFormDto dto)
        {
            var schoolId = GetSchoolId();

            var visitingForm = await _db.VisitingForms.FirstOrDefaultAsync(v => v.Id == id && v.SchoolId == schoolId && v.DeletedAt == null);

            if (visitingForm == null)
            {
                return NotFound(new { message = "訪視表不存在" });
            }

            visitingForm.Code = dto.Code;
            visitingForm.Class = dto.Class;
            visitingForm.Description = dto.Description;
            visitingForm.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await _historyService.Info(
                "修改訪視表",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(VisitingFormController),
                instanceKey: visitingForm.Id.ToString()
            );
            return NoContent();
        }

        //DELETE api/visitingForms/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Delete(int id)
        {
            var schoolId = GetSchoolId();
            var visitingForm = await _db.VisitingForms.FirstOrDefaultAsync(v => v.Id == id && v.SchoolId == schoolId && v.DeletedAt == null);
            if (visitingForm == null)
            {
                return NotFound(new { message = "訪視表不存在" });
            }

            visitingForm.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "刪除訪視表",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(VisitingFormController),
                instanceKey: visitingForm.Id.ToString()
            );
            return NoContent();
    }
}
}
