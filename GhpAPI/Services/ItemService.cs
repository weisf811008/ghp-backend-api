using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Services
{
    public class ItemService
    {

        private readonly AppDbContext _db;
        private readonly HistoryService _historyService;

        public ItemService(AppDbContext db, HistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        public async Task<List<ItemDto>> GetAll(int schoolId)
        {
            var items = await (
                   from i in _db.Items
                   join c in _db.Categories on i.CategoryId equals c.Id
                   where i.SchoolId == schoolId && i.DeletedAt == null
                   select new
                   {
                       i.Id,
                       i.No,
                       i.Item,
                       i.Period,
                       i.Area,
                       i.NeedCheckValue,
                       i.NeedDaily,
                       i.CategoryId,
                       CategoryName = c.Category,
                       i.SchoolId,
                       i.CreatedAt,
                       i.UpdatedAt,
                       i.DeletedAt
                   }
               ).ToListAsync();

            var itemIds = items.Select(i => i.Id).ToList();

            var itemRegulations = await (
                    from ir in _db.ItemRegulations
                    join r in _db.Regulations on ir.RegulationId equals r.Id
                    where itemIds.Contains(ir.ItemId)
                    select new
                    {
                        ir.ItemId,
                        RegulationCode = r.Code,
                    }
                ).ToListAsync();

            var itemVisitingFrom = await (
                    from iv in _db.ItemVisitingForms
                    join v in _db.VisitingForms on iv.VisitingFormId equals v.Id
                    where itemIds.Contains(iv.ItemId)
                    select new
                    {
                        iv.ItemId,
                        VisitingCode = v.Code,
                    }
                ).ToListAsync();

            var result = items.Select(i => new ItemDto
            {
                Id = i.Id,
                No = i.No,
                Item = i.Item,
                Period = i.Period,
                Area = i.Area,
                NeedCheckValue = i.NeedCheckValue,
                NeedDaily = i.NeedDaily,
                CategoryId = i.CategoryId,
                Regulations = itemRegulations
                    .Where(ir => ir.ItemId == i.Id)
                    .Select(ir => ir.RegulationCode)
                    .ToList(),
                VisitingForms = itemVisitingFrom
                    .Where(iv => iv.ItemId == i.Id)
                    .Select(iv => iv.VisitingCode)
                    .ToList(),
                CreatedAt = i.CreatedAt,
                UpdatedAt = i.UpdatedAt,
                DeletedAt = i.DeletedAt,
            }).ToList();

            return result;
        }

        public async Task<ItemDto?> GetById(int id, int schoolId)
        {
            var item = await (
                 from i in _db.Items
                 join c in _db.Categories on i.CategoryId equals c.Id
                 where i.Id == id && i.SchoolId == schoolId && i.DeletedAt == null
                 select new
                 {
                     i.Id,
                     i.No,
                     i.Item,
                     i.Period,
                     i.Area,
                     i.NeedCheckValue,
                     i.NeedDaily,
                     i.CategoryId,
                     CategoryName = c.Category,
                     i.CreatedAt,
                     i.UpdatedAt,
                     i.DeletedAt
                 }
                ).FirstOrDefaultAsync();

            if (item == null)
            {
                return null;
            }

            var ItemRegulations = await (
                    from ir in _db.ItemRegulations
                    join r in _db.Regulations on ir.RegulationId equals r.Id
                    where ir.ItemId == id
                    select new
                    {
                        RegulationCode = r.Code,
                    }
                ).ToListAsync();

            var ItemVisitingForms = await (
                    from iv in _db.ItemVisitingForms
                    join v in _db.VisitingForms on iv.VisitingFormId equals v.Id
                    where iv.ItemId == id
                    select new
                    {
                        VisitingFormCode = v.Code,
                    }
                ).ToListAsync();

            var result = new ItemDto
            {
                Id = item.Id,
                No = item.No,
                Item = item.Item,
                Period = item.Period,
                Area = item.Area,
                NeedCheckValue = item.NeedCheckValue,
                NeedDaily = item.NeedDaily,
                CategoryId = item.CategoryId,
                Regulations = ItemRegulations
                    .Select(ir => ir.RegulationCode)
                    .ToList(),
                VisitingForms = ItemVisitingForms
                    .Select(iv => iv.VisitingFormCode)
                    .ToList(),
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                DeletedAt = item.DeletedAt,
            };

            return result;
        }

        public async Task<(bool success, string? error, int? id)> Create(SaveItemDto dto, int schoolId, string username, string name)
        {
            var exist = await _db.Items.AnyAsync(i => i.No == dto.No && i.SchoolId == schoolId && i.DeletedAt == null);

            if (exist)
            {
                return (false, "細項已存在", null);
            }

            var item = new InspectionItem
            {
                No = dto.No,
                Item = dto.Item,
                Period = dto.Period,
                Area = dto.Area,
                NeedCheckValue = dto.NeedCheckValue,
                NeedDaily = dto.NeedDaily,
                CategoryId = dto.CategoryId,
                SchoolId = schoolId,
            };

            _db.Items.Add(item);
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "新增細項",
                username,
                name,
                schoolId,
                controller: nameof(ItemService),
                instanceKey: item.Id.ToString()
            );

            if (dto.Regulations != null && dto.Regulations.Count > 0)
            {
                var regulations = await _db.Regulations
                    .Where(r => dto.Regulations.Contains(r.Code) && r.SchoolId == schoolId && r.DeletedAt == null)
                    .ToListAsync();

                foreach (var regulation in regulations)
                {
                    _db.ItemRegulations.Add(new ItemRegulation
                    {
                        ItemId = item.Id,
                        RegulationId = regulation.Id,
                    });
                }
            }

            if (dto.VisitingForms != null && dto.VisitingForms.Count > 0)
            {
                var visitingForms = await _db.VisitingForms
                    .Where(v => dto.VisitingForms.Contains(v.Code) && v.SchoolId == schoolId && v.DeletedAt == null)
                    .ToListAsync();

                foreach (var visitingForm in visitingForms)
                {
                    _db.ItemVisitingForms.Add(new ItemVisitingForm
                    {
                        ItemId = item.Id,
                        VisitingFormId = visitingForm.Id,

                    });
                }
            }
            await _db.SaveChangesAsync();
            return (true, null, item.Id);
        }

        public async Task<(bool success, string? error)> Update(int id, SaveItemDto dto, int schoolId, string username, string name)
        {
            var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == id && i.SchoolId == schoolId && i.DeletedAt == null);

            if (item == null)
            {
                return (false, "細項不存在");
            }

            var exist = await _db.Items.AnyAsync(i => i.No == dto.No && i.SchoolId == schoolId && i.Id != id && i.DeletedAt == null);

            if (exist)
            {
                return (false, "細項編號已存在");
            }

            item.No = dto.No;
            item.Item = dto.Item;
            item.Period = dto.Period;
            item.Area = dto.Area;
            item.NeedCheckValue = dto.NeedCheckValue;
            item.NeedDaily = dto.NeedDaily;
            item.CategoryId = dto.CategoryId;
            item.UpdatedAt = DateTime.UtcNow;

            var existingRegulations = await _db.ItemRegulations
                .Where(ir => ir.ItemId == id)
                .ToListAsync();
            _db.ItemRegulations.RemoveRange(existingRegulations);

            var existingVisitingForms = await _db.ItemVisitingForms
                .Where(iv => iv.ItemId == id)
                .ToListAsync();
            _db.ItemVisitingForms.RemoveRange(existingVisitingForms);

            if (dto.Regulations != null && dto.Regulations.Count > 0)
            {
                var regulations = await _db.Regulations
                    .Where(r => dto.Regulations.Contains(r.Code) && r.SchoolId == schoolId && r.DeletedAt == null)
                    .ToListAsync();

                foreach (var regulation in regulations)
                {
                    _db.ItemRegulations.Add(new ItemRegulation
                    {
                        ItemId = item.Id,
                        RegulationId = regulation.Id,
                    });
                }
            }

            if (dto.VisitingForms != null && dto.VisitingForms.Count > 0)
            {
                var visitingForms = await _db.VisitingForms
                    .Where(v => dto.VisitingForms.Contains(v.Code) && v.SchoolId == schoolId && v.DeletedAt == null)
                    .ToListAsync();

                foreach (var visitingForm in visitingForms)
                {
                    _db.ItemVisitingForms.Add(new ItemVisitingForm
                    {
                        ItemId = item.Id,
                        VisitingFormId = visitingForm.Id,
                    });
                }
            }

            await _db.SaveChangesAsync();
            await _historyService.Info(
                "修改細項",
                username,
                name,
                schoolId,
                controller: nameof(ItemService),
                instanceKey: item.Id.ToString()
            );

            return (true, null);
        }

        public async Task<(bool success, string? error)> Delete(int id, int schoolId, string username, string name)
        {
            var  item = await _db.Items.FirstOrDefaultAsync(i => i.Id == id && i.SchoolId == schoolId && i.DeletedAt == null);

            if (item == null)
            {
                return (false, "細項不存在");
            }

            item.DeletedAt = DateTime.Now;

            var existingRegulations = await _db.ItemRegulations
                .Where(ir => ir.ItemId == id)
                .ToListAsync();

            _db.ItemRegulations.RemoveRange(existingRegulations);

            var existingVisitingForms = await _db.ItemVisitingForms
                .Where(iv => iv.ItemId == id)
                .ToListAsync();
            _db.ItemVisitingForms.RemoveRange(existingVisitingForms);

            await _db.SaveChangesAsync();
            await _historyService.Info(
                "刪除細項",
                username,
                name,
                schoolId,
                controller: nameof(ItemService),
                instanceKey: item.Id.ToString()
            );

            return (true, null);
        }
    }
}
