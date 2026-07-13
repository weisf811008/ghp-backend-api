using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/forms")]
    [ApiController]
    [Authorize]
    [Tags("表單管理")]

    public class FormsController : BaseController
    {
        public FormsController(AppDbContext db, HistoryService historyService)
       : base(db, historyService)
        {
        }

        //GET api/forms
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetAll()
        {
            var schoolId = GetSchoolId();

            var forms = await _db.Forms
                .Where(f => f.SchoolId == schoolId && f.DeletedAt == null)
                .Select(f => new FormDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Remarks = f.Remarks,
                    SchoolId = f.SchoolId,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    DeletedAt = f.DeletedAt
                }).ToListAsync();

            return Ok(forms);
        }

        //GET api/forms/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetById(int id)
        {
            var schoolId = GetSchoolId();

            var form = await _db.Forms
                .Where(f => f.SchoolId == schoolId && f.Id == id && f.DeletedAt == null)
                .Select(f => new FormDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    Remarks = f.Remarks,
                    SchoolId = f.SchoolId,
                    CreatedAt = f.CreatedAt,
                    UpdatedAt = f.UpdatedAt,
                    DeletedAt = f.DeletedAt
                }).FirstOrDefaultAsync();

            if (form == null)
            {
                return NotFound(new { message = "表單不存在" });
            }

            var formDetails = await (
                    from fd in _db.FormDetails
                    join i in _db.Items on fd.ItemId equals i.Id
                    join c in _db.Categories on i.CategoryId equals c.Id
                    where fd.FormId == id
                    select new FormDetailDto
                    {
                        ItemId = fd.ItemId,
                        No = i.No,
                        Item = i.Item,
                        NeedCheckValue = i.NeedCheckValue,
                        CategoryId = i.CategoryId,
                        Category = c.Category,
                    }
                ).ToListAsync();

            var result = new FormDto
            {
                Id = form.Id,
                Title = form.Title,
                Remarks = form.Remarks,
                SchoolId = form.SchoolId,
                Details = formDetails,
                CreatedAt = form.CreatedAt,
                UpdatedAt = form.UpdatedAt,
                DeletedAt = form.DeletedAt
            };

            return Ok(result);
        }

        //POST api/forms
        [HttpPost]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Create([FromBody] SaveFormDto dto)
        {
            var schoolId = GetSchoolId();
            var exist = await _db.Forms.AnyAsync(f => f.Title == dto.Title && f.SchoolId == schoolId && f.DeletedAt == null);

            if (exist)
            {
                return Conflict(new { message = "表單已存在" });
            }

            var form = new Form
            {
                Title = dto.Title,
                Remarks = dto.Remarks,
                SchoolId = schoolId,
            };

            _db.Forms.Add(form);
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "新增表單",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(FormsController),
                instanceKey: form.Id.ToString()
            );


            if (dto.ItemIds != null && dto.ItemIds.Count > 0)
            {
                foreach (var itemId in dto.ItemIds)
                {
                    _db.FormDetails.Add(new FormDetail
                    {
                        FormId = form.Id,
                        ItemId = itemId,
                    });
                }
                await _db.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetById), new { id = form.Id }, new { id = form.Id });
        }

        //PUT api/forms/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "學校管理員")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveFormDto dto)
        {
            var schoolId = GetSchoolId();
            var form = await _db.Forms.FirstOrDefaultAsync(f => f.Id == id && f.SchoolId == schoolId && f.DeletedAt == null);

            if (form == null)
            {
                return NotFound(new { message = "表單不存在" });
            }

            form.Title = dto.Title;
            form.Remarks = dto.Remarks;
            form.UpdatedAt = DateTime.UtcNow;

            var existingDetails = await _db.FormDetails
                .Where(fd => fd.FormId == id)
                .ToListAsync();

            _db.FormDetails.RemoveRange(existingDetails);

            if (dto.ItemIds != null && dto.ItemIds.Count > 0)
            {
                foreach (var itemId in dto.ItemIds)
                {
                    _db.FormDetails.Add(new FormDetail
                    {
                        FormId = form.Id,
                        ItemId = itemId,
                    });
                }
            }

            await _db.SaveChangesAsync();
            await _historyService.Info(
                "修改表單",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(FormsController),
                instanceKey: form.Id.ToString()
            );
            return NoContent();
        }

        //DELETE api/forms/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Delete(int id)
        {
            var schoolId = GetSchoolId();

            var form = await _db.Forms.FirstOrDefaultAsync(f => f.Id == id && f.SchoolId == schoolId && f.DeletedAt == null);

            if (form == null)
            {
                return NotFound(new { message = "表單不存在" });
            }

            form.DeletedAt = DateTime.UtcNow;

            var existingDetails = await _db.FormDetails
                .Where(fd => fd.FormId == id)
                .ToListAsync();

            await _db.SaveChangesAsync();
            await _historyService.Info(
                "刪除表單",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(FormsController),
                instanceKey: form.Id.ToString()
            );
            return NoContent();
        }
    }
}
