using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Services
{
    public class VisitingFormService
    {
        private readonly AppDbContext _db;
        private readonly HistoryService _historyService;

        public VisitingFormService(AppDbContext db, HistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        public async Task<List<VisitingFormDto>> GetAll(int schoolId)
        {
            return await _db.VisitingForms
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
        }

        public async Task<VisitingFormDto?> GetById(int id, int schoolId)
        {
            return await _db.VisitingForms
                .Where(v => v.Id == id && v.SchoolId == schoolId && v.DeletedAt == null)
                .Select(v => new VisitingFormDto
                {
                    Id = v.Id,
                    Code = v.Code,
                    Class = v.Class,
                    Description = v.Description,
                    CreatedAt = v.CreatedAt,
                    UpdatedAt = v.UpdatedAt,
                    DeletedAt = v.DeletedAt,
                }).FirstOrDefaultAsync();
        }

        public async Task<(bool success, string? error, int? id)> Create(SaveVisitingFormDto dto, int schoolId, string username, string name)
        {
            var exist = await _db.VisitingForms.AnyAsync(v => v.Code == dto.Code && v.SchoolId == schoolId && v.DeletedAt == null);

            if (exist)
            {
                return (false, "訪視表已存在", null);
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
                username,
                name,
                schoolId,
                controller: nameof(VisitingFormService),
                instanceKey: visitingForm.Id.ToString()
            );
            return (true, null, visitingForm.Id);
        }

        public async Task<(bool success, string? error)> Update(int id, SaveVisitingFormDto dto, int schoolId, string username, string name)
        {
            var visitingForm = await _db.VisitingForms.FirstOrDefaultAsync(v => v.Id == id && v.SchoolId == schoolId && v.DeletedAt == null);

            if (visitingForm == null)
            {
                return (false, "訪視表不存在");
            }

            var exist = await _db.VisitingForms.AnyAsync(v => v.Code == dto.Code && v.SchoolId == schoolId && v.DeletedAt == null && v.Id != id);

            if (exist)
            {
                return (false, "訪視表已存在");
            }
            visitingForm.Code = dto.Code;
            visitingForm.Class = dto.Class;
            visitingForm.Description = dto.Description;
            visitingForm.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _historyService.Info(
                "更新訪視表",
                username,
                name,
                schoolId,
                controller: nameof(VisitingFormService),
                instanceKey: visitingForm.Id.ToString()
            );
            return (true, null);
        }

        public async Task<(bool success, string? error)> Delete(int id, int schoolId, string username, string name)
        {
            var visitingForm = await _db.VisitingForms.FirstOrDefaultAsync(v => v.Id == id && v.SchoolId == schoolId && v.DeletedAt == null);

            if (visitingForm == null)
            {
                return (false, "訪視表不存在");
            }

            visitingForm.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _historyService.Info(
                "刪除訪視表",
                username,
                name,
                schoolId,
                controller: nameof(VisitingFormService),
                instanceKey: visitingForm.Id.ToString()
            );
            return (true, null);
        }
    }
}
