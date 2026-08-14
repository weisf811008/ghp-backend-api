using GhpAPI.Data;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/histories")]
    [ApiController]
    [Tags("操作紀錄")]
    [Authorize(Roles = "學校管理員")]
    public class HistoriesController : BaseController
    {
        public HistoriesController(AppDbContext db, HistoryService historyService)
            : base(db, historyService)
        {
        }

        private int GetSchoolId()
        {
            return int.Parse(User.FindFirst("schoolId")!.Value);
        }

        //GET api/histories
        [HttpGet]
        public async Task<IActionResult> GetAll(DateTime? start, DateTime? end)
        {
            var endDate = end ?? DateTime.Now;
            var startDate = start ?? endDate.AddDays(-7);

            var result = await _historyService.GetAll(GetSchoolId(), startDate, endDate);
            return Ok(result);
        }
    }
}
