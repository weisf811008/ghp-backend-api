using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/tags")]
    [ApiController]
    [Tags("標籤管理")]
    public class TagsController : ControllerBase
    {
        private static readonly string[] Periods = ["半年(開學前)", "日", "週"];
        private static readonly string[] Areas = ["人員衛生", "作業區", "烹調區", "配膳區", "庫房"];


        //GET api/tags/periods
        [HttpGet("periods")]

        public IActionResult GetPeriods()
        {
            return Ok(Periods);
        }

        // GET api/tags/areas
        [HttpGet("areas")] 
        public IActionResult GetAreas()
        {
            return Ok(Areas);
        }
    }
}
