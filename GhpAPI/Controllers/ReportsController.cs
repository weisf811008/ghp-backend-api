using GhpAPI.Data;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/reports")]
    [ApiController]
    [Tags("報表管理")]
    [Authorize(Roles = "學校管理員,巡檢人員")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ReportService _reportService;

        public ReportsController(AppDbContext db, ReportService reportService)
        {
            _db = db;
            _reportService = reportService;
        }

        private int GetSchoolId()
        {
            return int.Parse(User.FindFirst("schoolId")!.Value);
        }

        // GET api/reports/daily?start=2026-01-01&end=2026-01-31
        [HttpGet("daily")]
        public async Task<IActionResult> GetDailyReport(DateTime start, DateTime end)
        {
            var result = await _reportService.GetDailyReport(GetSchoolId(), start, end);
            return Ok(result);
        }

        // GET api/reports/ghp?start=2026-01-01&end=2026-01-31
        [HttpGet("ghp")]
        public async Task<IActionResult> GetGhpReport(DateTime start, DateTime end)
        {
            var result = await _reportService.GetGhpReport(GetSchoolId(), start, end);
            return Ok(result);
        }

        // GET api/reports/visiting?start=2026-01-01&end=2026-01-31
        [HttpGet("visiting")]
        public async Task<IActionResult> GetVisitingReport(DateTime start, DateTime end)
        {
            var result = await _reportService.GetVisitingReport(GetSchoolId(), start, end);
            return Ok(result);
        }

        // GET api/reports/prodtemp?start=2026-01-01&end=2026-01-31
        [HttpGet("prodtemp")]
        public async Task<IActionResult> GetProdTempReport(DateTime start, DateTime end)
        {
            var result = await _reportService.GetProdTempReport(GetSchoolId(), start, end);
            return Ok(result);
        }

        // GET api/reports/tnh?start=2026-01-01&end=2026-01-31
        [HttpGet("tnh")]
        public async Task<IActionResult> GetTnhReport(DateTime start, DateTime end)
        {
            var result = await _reportService.GetTnhReport(GetSchoolId(), start, end);
            return Ok(result);
        }

        // GET api/reports/tableware?start=2026-01-01&end=2026-01-31
        [HttpGet("tableware")]
        public async Task<IActionResult> GetTablewareReport(DateTime start, DateTime end)
        {
            var result = await _reportService.GetTablewareReport(GetSchoolId(), start, end);
            return Ok(result);
        }
    }
}