using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/inspections")]
    [ApiController]
    [Tags("巡檢紀錄")]

    public class InspectionsController : BaseController
    {
        private readonly InspectionService _inspectionService;
        private readonly IWebHostEnvironment _env;
        public InspectionsController(AppDbContext db, HistoryService historyService, InspectionService inspectionService, IWebHostEnvironment env)
        : base(db, historyService)
        {
            _env = env;
            _inspectionService = inspectionService;
        }


        //GET api/inspections
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetAll()
        {
            var result = await _inspectionService.GetAll(GetSchoolId());

            return Ok(result);
        }

        //GET api/inspection/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetById(int id)
        {
            var result = await _inspectionService.GetById(id, GetSchoolId());

            if (result == null)
            {
                return NotFound(new { message = "巡檢紀錄不存在" });
            }

            return Ok(result);
        }

        //POST api/inspections
        [HttpPost]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> Create([FromBody] SaveInspectionDto dto)
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var (success, error, id) = await _inspectionService.Create(dto, GetSchoolId(), userId, GetUsername(), GetName());

            if (!success)
            {
                return Conflict(new { message = error });
            }

            return CreatedAtAction(nameof(GetById), new { id }, new { id });
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

        //GET api/inspections/files/{filename}
        [HttpGet("files/{filename}")]

        public IActionResult GetFile(string filename)
        {
            var uploadDir = Path.Combine(_env.ContentRootPath, "uploads", "insp", "files");
            var filePath = Path.Combine(uploadDir, filename);

            var fullPath = Path.GetFullPath(filePath);
            if (!fullPath.StartsWith(Path.GetFullPath(uploadDir)))
            {
                return BadRequest(new { message = "無效的檔案路徑" });
            }

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "檔案不存在" });
            }

            var mimeType = GetMimeType(filename);
            return PhysicalFile(filePath, mimeType);
        }

        private string GetMimeType(string filename)
        {
            var ext = Path.GetExtension(filename).ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
    }
}
