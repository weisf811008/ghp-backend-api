using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Services
{
    public class SchoolService
    {
        private readonly AppDbContext _db;
        private readonly HistoryService _historyService;

        public SchoolService(AppDbContext db, HistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        public async Task<List<SchoolDetailDto>> GetAll()
        {
            return await _db.Schools
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
                }).ToListAsync();
        }

        public async Task<SchoolDetailDto?> GetById(int id)
        {
            return await _db.Schools
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
                }).FirstOrDefaultAsync();
        }

        public async Task<(bool success, string? error, int? id)> Create(CreateSchoolDto dto, string username, string name)
        {
            var exist = await _db.Schools.AnyAsync(s => s.Code == dto.Code && s.DeletedAt == null);
            if (exist) return (false, "學校編號已存在", null);

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
                username,
                name,
                school.Id,
                controller: nameof(SchoolService),
                instanceKey: school.Id.ToString()
            );

            return (true, null, school.Id);
        }

        public async Task<(bool success, string? error)> Update(int id, UpdateSchoolDto dto, string username, string name)
        {
            var school = await _db.Schools.FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null);
            if (school == null) return (false, "學校不存在");

            school.Name = dto.Name!;
            school.City = dto.City;
            school.Address = dto.Address;
            school.Phone = dto.Phone;
            school.Url = dto.Url;
            school.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await _historyService.Info(
                "修改學校",
                username,
                name,
                school.Id,
                controller: nameof(SchoolService),
                instanceKey: school.Id.ToString()
            );

            return (true, null);
        }

        public async Task<(bool success, string? error)> Delete(int id, string username, string name)
        {
            var school = await _db.Schools.FirstOrDefaultAsync(s => s.Id == id && s.DeletedAt == null);
            if (school == null) return (false, "學校不存在");

            school.DeletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            await _historyService.Info(
                "刪除學校",
                username,
                name,
                school.Id,
                controller: nameof(SchoolService),
                instanceKey: school.Id.ToString()
            );

            return (true, null);
        }
    }
}