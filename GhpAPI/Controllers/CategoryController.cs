using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    [Tags("大項管理")]
    public class CategoryController : BaseController
    {
        public CategoryController(AppDbContext db, HistoryService historyService)
            : base(db, historyService)
        {
        }

        // GET api/categories
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]
        public async Task<IActionResult> GetAll()
        {

            var schoolId = GetSchoolId();

            var categories = await _db.Categories
                .Where(c => c.SchoolId == schoolId && c.DeletedAt == null)
                .Select(c => new CategoryDTo
                {
                    Id = c.Id,
                    Category = c.Category,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    DeletedAt = c.DeletedAt
                }).ToListAsync();
            return Ok(categories);
        }

        // GET api/categories/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetById(int id)
        {
            var schoolId = GetSchoolId();

            var category = await _db.Categories
                .Where(c => c.Id == id && c.SchoolId == schoolId && c.DeletedAt == null)
                .Select(c => new CategoryDTo
                {
                    Id = c.Id,
                    Category = c.Category,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    DeletedAt = c.DeletedAt
                }).FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { message = "大項不存在" });
            }

            return Ok(category);
        }

        //POST api/categories
        [HttpPost]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Create([FromBody] SaveCategoryDto dto)
        {
            var schoolId = GetSchoolId();
            var exist = await _db.Categories.AnyAsync(c => c.Category == dto.Category && c.SchoolId == schoolId && c.DeletedAt == null);

            if (exist)
            {
                return Conflict(new { message = "大項已存在" });
            }

            var category = new CategoryItem
            {
                Category = dto.Category,
                SchoolId = schoolId,
            };

            _db.Categories.Add(category);
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "新增大項",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(CategoryController),
                instanceKey: category.Id.ToString()
            );

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, new { id = category.Id });
        }

        //PUT api/categories/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "學校管理員")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveCategoryDto dto)
        {
            var schoolId = GetSchoolId();

            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id && c.SchoolId == schoolId && c.DeletedAt == null);

            if (category == null)
            {
                return NotFound(new { message = "大項不存在" });
            }

            category.Category = dto.Category;
            category.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await _historyService.Info(
                "修改大項",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(CategoryController),
                instanceKey: id.ToString()
            );

            return NoContent();
        }

        //DELETE api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "學校管理員")]
        public async Task<IActionResult> Delete(int id)
        {
            var schoolId = GetSchoolId();

            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id && c.SchoolId == schoolId && c.DeletedAt == null);

            if (category == null)
            {
                return NotFound(new { message = "大項不存在" });
            }

            category.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "刪除大項",
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(CategoryController),
                instanceKey: id.ToString()
            );

            return NoContent();
        }
    }
}
