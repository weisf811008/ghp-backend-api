using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Services
{
    public class InspectionService
    {
        private readonly AppDbContext _db;
        private readonly HistoryService _historyService;

        public InspectionService(AppDbContext db, HistoryService historyService)
        {
            _db = db;
            _historyService = historyService;
        }

        public async Task<List<InspectionDto>> GetAll(int schoolId)
        {
            var result  = await (
                    from i in _db.Inspections
                    join f in _db.Forms on i.FormId equals f.Id
                    join u in _db.Users on i.InspectedBy equals u.Id
                    where i.SchoolId == schoolId
                    select new InspectionDto
                    {
                        Id = i.Id,
                        Date = i.Date,
                        DueDate = i.DueDate,
                        Remarks = i.Remarks,
                        Version = i.Version,
                        ClosedAt = i.ClosedAt,
                        ParentId = i.ParentId,
                        FormId = i.FormId,
                        Title = f.Title,
                        CreatedAt = i.CreatedAt,
                        InspectedBy = new InspectedByDto
                        {
                            Username = u.Username,
                            Name = u.Name,
                        }
                    }).ToListAsync();

            return result;
        }

        public async Task<InspectionWithDetailsDto?> GetById(int id, int schoolId)
        {
            var inspection = await (
                from i in _db.Inspections
                join f in _db.Forms on i.FormId equals f.Id
                join u in _db.Users on i.InspectedBy equals u.Id
                where i.SchoolId == schoolId && i.Id == id
                select new InspectionWithDetailsDto
                {
                    Id = i.Id,
                    Date = i.Date,
                    DueDate = i.DueDate,
                    Remarks = i.Remarks,
                    Version = i.Version,
                    ClosedAt = i.ClosedAt,
                    ParentId = i.ParentId,
                    FormId = i.FormId,
                    Title = f.Title,
                    CreatedAt = i.CreatedAt,
                    InspectedBy = new InspectedByDto
                    {
                        Username = u.Username,
                        Name = u.Name,
                    }
                }).FirstOrDefaultAsync();

            if (inspection == null)
            {
                return null;
            }

            var details = await (
                from ind in _db.InspectionDetails
                join item in _db.Items on ind.ItemId equals item.Id
                join c in _db.Categories on item.CategoryId equals c.Id
                where ind.InspectionId == id
                select new InspectionDetailDto
                {
                    ItemId = ind.ItemId,
                    No = item.No,
                    Item = item.Item,
                    Category = c.Category,
                    NeedCheckValue = item.NeedCheckValue,
                    Status = ind.Status,
                    Remarks = ind.Remarks,
                    CheckValue = ind.CheckValue,
                }).ToListAsync();

            var files = await _db.InspectionFiles
                .Where(f => f.InspectionId == id)
                .Select(f => new InspectionFileDto
                {
                    Id = f.Id,
                    ItemId = f.ItemId,
                    Filename = f.Filename,
                    Originalname = f.Originalname,
                    Encoding = f.Encoding,
                    Mimetype = f.Mimetype,
                }).ToListAsync();

            foreach (var detail in details)
            {
                detail.Files = files
                    .Where(f => f.ItemId == detail.ItemId)
                    .ToList();
            }

            var result = new InspectionWithDetailsDto
            {
                Id = inspection.Id,
                Date = inspection.Date,
                DueDate = inspection.DueDate,
                Remarks = inspection.Remarks,
                Version = inspection.Version,
                ClosedAt = inspection.ClosedAt,
                ParentId = inspection.ParentId,
                FormId = inspection.FormId,
                Title = inspection.Title,
                CreatedAt = inspection.CreatedAt,
                InspectedBy = inspection.InspectedBy,
                Details = details,
            };
            return result;
        }

        public async Task<(bool success, string? error, int? id)> Create(SaveInspectionDto dto, int schoolId, int userId, string username, string name)
        {
            var version = await _db.Inspections
                .Where(i => i.FormId == dto.FormId && i.SchoolId == schoolId && i.Date == dto.Date)
                .CountAsync() + 1;

            var inspection = new Inspection
            {
                Date = dto.Date,
                DueDate = dto.DueDate,
                Remarks = dto.Remarks,
                FormId = dto.FormId,
                Version = version,
                SchoolId = schoolId,
                InspectedBy = userId,
            };

            _db.Inspections.Add(inspection);
            await _db.SaveChangesAsync();

            foreach (var detail in dto.Details)
            {
                var inspectionDetail = new InspectionDetail
                {
                    InspectionId = inspection.Id,
                    ItemId = detail.ItemId,
                    Status = detail.Status,
                    Remarks = detail.Remarks,
                    CheckValue = detail.CheckValue,
                };

                _db.InspectionDetails.Add(inspectionDetail);

                if (detail.Files != null && detail.Files.Count > 0)
                {
                    foreach (var file in detail.Files)
                    {
                        _db.InspectionFiles.Add(new InspectionFile
                        {
                            Filename = file.Filename,
                            Originalname = file.Originalname,
                            Encoding = file.Encoding,
                            Mimetype = file.Mimetype,
                            InspectionId = inspection.Id,
                            ItemId = detail.ItemId,
                        });
                    }
                }
            }
            await _db.SaveChangesAsync();
            await _historyService.Info(
                "新增巡檢紀錄",
                username,
                name,
                schoolId,
                controller: nameof(InspectionService),
                instanceKey: inspection.Id.ToString()
            );

            return (true, null, inspection.Id);
        }
    }
}
