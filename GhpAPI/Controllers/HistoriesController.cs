using GhpAPI.Data;
using GhpAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/histories")]
    [ApiController]
    [Tags("操作紀錄")]
    [Authorize(Roles = "學校管理員")]
    public class HistoriesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public HistoriesController(AppDbContext db) {
            _db = db;
        }

        private int GetSchoolId()
        {
            return int.Parse(User.FindFirst("schoolId")!.Value);
        }

        //GET api/histories
        [HttpGet]
        public async Task<IActionResult> GetAll(DateTime? start, DateTime? end)
        { 
            var schoolId = GetSchoolId();

            var endDate = end ?? DateTime.Now;
            var startDate = start ?? endDate.AddDays(-7);

            var histories = await _db.Histories
                .Where(h => h.SchoolId == schoolId && h.Timestamp >= startDate && h.Timestamp <=endDate)
                .OrderByDescending(h => h.Timestamp)
                .Take(500)
                .Select(h => new HistoryDto
                {
                    Id = h.Id,
                    Timestamp = h.Timestamp,
                    Level = h.Level,
                    Message = h.Message,
                    Type = h.Type,
                    Controller = h.Controller,
                    InstanceKey = h.InstanceKey,
                    Username = h.Username,
                    Name = h.Name,
                    SchoolId = h.SchoolId,
                }).ToListAsync();

            return Ok(histories);
        }
            
    }
}
