using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Services
{
    public class RegulationService
    {
        private readonly AppDbContext _db;
        private readonly HistoryService _historyService;

        public RegulationService(AppDbContext db, HistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        public async Task<List<RegulationDto>> GetAll(int schoolId)
        {
            return await _db.Regulations
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
        }

        public async Task<RegulationDto?> GetById(int id, int schoolId)
        {
            return await _db.Regulations
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
        }

        public async Task<(bool success, string? error, int? id)> Create(SaveRegulationDto dto, int schoolId, string username, string name)
        {
            var exist = await _db.Regulations.AnyAsync(r => r.Code == dto.Code && r.SchoolId == schoolId && r.DeletedAt == null);
            if (exist)
            {
                return (false, "條文已存在", null);
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
               username,
               name,
               schoolId,
               controller: nameof(RegulationService),
               instanceKey: regulation.Id.ToString()
           );
            return (true, null, regulation.Id);
        }

        public async Task<(bool success, string? error)> Update(int id, SaveRegulationDto dto, int schoolId, string username, string name)
        {
            var regulation = await _db.Regulations.FirstOrDefaultAsync(r => r.Id == id && r.SchoolId == schoolId && r.DeletedAt == null);

            if (regulation == null)
            {
                return (false, "條文不存在");
            }

            var exist = await _db.Regulations.AnyAsync(r => r.Code == dto.Code && r.SchoolId == schoolId && r.Id != id && r.DeletedAt == null);

            if (exist)
            {
                return (false, "條文編號已存在");
            }

            regulation.Code = dto.Code;
            regulation.Class = dto.Class;
            regulation.Description = dto.Description;
            regulation.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _historyService.Info(
                "修改條文",
                username,
                name,
                schoolId,
                controller: nameof(RegulationService),
                instanceKey: regulation.Id.ToString()
            );

            return (true, null);
        }

        public async Task<(bool success, string? error)> Delete(int id, int schoolId, string username, string name)
        {
            var regulation = await _db.Regulations.FirstOrDefaultAsync(r => r.Id == id && r.SchoolId == schoolId && r.DeletedAt == null);
            if (regulation == null)
            {
                return (false, "條文不存在");
            }

            regulation.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _historyService.Info(
                "刪除條文",
                username,
                name,
                schoolId,
                controller: nameof(RegulationService),
                instanceKey: regulation.Id.ToString()
            );

            return (true, null);
        }
    }
}
