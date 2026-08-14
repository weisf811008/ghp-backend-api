using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Services
{
    public class CategoryService
    {
        private readonly AppDbContext _db;
        private readonly HistoryService _historyService;

        public CategoryService(AppDbContext db, HistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        public async Task<List<CategoryDTo>> GetAll(int schoolId)
        {
            return await _db.Categories
                .Where(c => c.SchoolId == schoolId && c.DeletedAt == null)
                .Select(c => new CategoryDTo
                {
                    Id = c.Id,
                    Category = c.Category,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    DeletedAt = c.DeletedAt
                }).ToListAsync();
        }

        public async Task<CategoryDTo?> GetById(int id, int schoolId)
        {
            return await _db.Categories
                .Where(c => c.Id == id && c.SchoolId == schoolId && c.DeletedAt == null)
                .Select(c => new CategoryDTo
                {
                    Id = c.Id,
                    Category = c.Category,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    DeletedAt = c.DeletedAt
                }).FirstOrDefaultAsync();
        }

        public async Task<(bool success, string? error, int? id)> Create(SaveCategoryDto dto, int schoolId, string username, string name)
        {
            var exist = await _db.Categories.AnyAsync(c => c.Category == dto.Category && c.SchoolId == schoolId && c.DeletedAt == null);

            if (exist)
            {
                return (false, "大項已存在", null);
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
                username,
                name,
                schoolId,
                controller: nameof(CategoryService),
                instanceKey: category.Id.ToString()
            );
            return (true, null, category.Id);
        }

        public async Task<(bool success, string? error)> Update(int id, SaveCategoryDto dto, int schoolId, string username, string name)
        {
            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id && c.SchoolId == schoolId && c.DeletedAt == null);
            if (category == null) return (false, "大項不存在");

            category.Category = dto.Category;
            category.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _historyService.Info(
                "修改大項",
                username: username,
                name: name,
                schoolId: schoolId,
                controller: nameof(CategoryService),
                instanceKey: id.ToString()
            );

            return (true, null);
        }

        public async Task<(bool success, string? error)> Delete(int id, int schoolId, string username, string name)
        {
            var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id && c.SchoolId == schoolId && c.DeletedAt == null);
            if (category == null) return (false, "大項不存在");

            category.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _historyService.Info(
                "刪除大項",
                username: username,
                name: name,
                schoolId: schoolId,
                controller: nameof(CategoryService),
                instanceKey: id.ToString()
            );

            return (true, null);
        }
    }
}
