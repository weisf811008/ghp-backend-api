using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Services
{
    public class FormService
    {
        private readonly AppDbContext _db;
        private readonly HistoryService _historyService;

        public FormService(AppDbContext db, HistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        public async Task<List<FormDto>> GetAll(int schoolId)
        {
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

            return forms;
        }

        public async Task<FormDto?> GetById(int id, int schoolId)
        {
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
                return null;
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
            return result;
        }

        public async Task<(bool success, string? error, int? id)> Create(SaveFormDto dto, int schoolId, string username, string name)
        {
            var exist = await _db.Forms.AnyAsync(f => f.Title == dto.Title && f.SchoolId == schoolId && f.DeletedAt == null);

            if (exist)
            {
                return (false, "表單已存在", null);
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
                username,
                name,
                schoolId,
                controller: nameof(FormService),
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

            return (true, null, form.Id);
        }

        public async Task<(bool success, string? error)> Update(int id, SaveFormDto dto, int schoolId, string username, string name)
        {
            var form = await _db.Forms.FirstOrDefaultAsync(f => f.Id == id && f.SchoolId == schoolId && f.DeletedAt == null);

            if (form == null)
            {
                return (false, "表單不存在");
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
                username,
                name,
                schoolId,
                controller: nameof(FormService),
                instanceKey: form.Id.ToString()
            );
            return (true, null);
        }

        public async Task<(bool success, string? error)> Delete(int id, int schoolId, string username, string name)
        {
            var form = await _db.Forms.FirstOrDefaultAsync(f => f.Id == id && f.SchoolId == schoolId && f.DeletedAt == null);

            if (form == null)
            {
                return (false, "表單不存在");
            }

            form.DeletedAt = DateTime.UtcNow;

            var existingDetails = await _db.FormDetails
                .Where(fd => fd.FormId == id)
                .ToListAsync();

            _db.FormDetails.RemoveRange(existingDetails);
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "刪除表單",
                username,
                name,
                schoolId,
                controller: nameof(FormService),
                instanceKey: form.Id.ToString()
            );

            return (true, null);
        }
    }
}
