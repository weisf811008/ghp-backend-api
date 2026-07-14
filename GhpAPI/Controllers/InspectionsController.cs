using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/inspections")]
    [ApiController]
    [Tags("巡檢紀錄")]
    [Authorize]
    public class InspectionsController : BaseController
    {
        private readonly IWebHostEnvironment _env;
        public InspectionsController(AppDbContext db, HistoryService historyService, IWebHostEnvironment env)
        : base(db, historyService)
        {
            _env = env;
        }


        //GET api/inspections
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetAll()
        {
            var schoolId = GetSchoolId();

            var Inspections = await (
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

            return Ok(Inspections);
        }

        //GET api/inspection/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetById(int id)
        {
            var schoolId = GetSchoolId();

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
                return NotFound(new { message = "巡檢紀錄不存在" });
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

            return Ok(result);
        }

        //POST api/inspections
        [HttpPost]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> Create([FromBody] SaveInspectionDto dto)
        {
            var schoolId = GetSchoolId();

            var exist = await _db.Inspections.AnyAsync(i => i.FormId == dto.FormId && i.SchoolId == schoolId && i.Date == dto.Date);

            if (exist)
            {
                return Conflict(new { message = "巡檢紀錄已存在" });
            }

            var inspection = new Inspection
            {
                Date = dto.Date,
                DueDate = dto.DueDate,
                Remarks = dto.Remarks,
                FormId = dto.FormId,
                Version = 1,
                SchoolId = schoolId,
                InspectedBy = int.Parse(User.FindFirst("id")!.Value),
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
                username: GetUsername(),
                name: GetName(),
                schoolId: schoolId,
                controller: nameof(InspectionsController),
                instanceKey: inspection.Id.ToString()
            );
            return CreatedAtAction(nameof(GetById), new { id = inspection.Id }, new { id = inspection.Id });
        }

        //POST api/inspections/files
        [HttpPost("files")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "請選擇檔案" });
            }

            var uploadDir = Path.Combine(_env.ContentRootPath, "uploads", "insp", "files");

            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            var ext = Path.GetExtension(file.FileName);
            var filename = $"{DateTime.Now:yyyyMMddHHmmss}{new Random().Next(1000000000)}{ext}";
            var filePath = Path.Combine(uploadDir, filename);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            await _historyService.Info(
                "上傳巡檢附檔",
                username: GetUsername(),
                name: GetName(),
                schoolId: GetSchoolId(),
                controller: nameof(InspectionsController),
                instanceKey: filename
            );

            return StatusCode(201, new
            {
                filename,
                originalname = file.FileName,
                encoding = "7bit",
                mimetype = file.ContentType,
            });
        }
    }
}
