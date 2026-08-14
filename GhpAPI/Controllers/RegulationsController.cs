using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/regulations")]
    [ApiController]
    [Tags("條文管理")]
   
    public class RegulationsController : BaseController
    {
        private readonly RegulationService _regulationService;
        public RegulationsController(AppDbContext db, HistoryService historyService, RegulationService regulationService)
         : base(db, historyService)
        {
            _regulationService = regulationService;
        }

        //GET api/regulations
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetAll()
        {
            var regulations = await _regulationService.GetAll(GetSchoolId());
            return Ok(regulations);
        }

        //GET api/regulations/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _regulationService.GetById(id, GetSchoolId());

            if (result == null)
            {
                return NotFound(new { message = "條文不存在" });
            }
            return Ok(result);
        }


        //POST api/regulations
        [HttpPost]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Create([FromBody] SaveRegulationDto dto)
        {
            var (success, error, id) = await _regulationService.Create(dto, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                return Conflict(new { message = error });
            }

            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        //PUT api/regulations/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Update(int id, [FromBody] SaveRegulationDto dto)
        {
            var (success, error) = await _regulationService.Update(id, dto, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                if (error == "條文不存在")
                {
                    return NotFound(new { message = error });
                }

                return Conflict(new { message = error });
            }

            return NoContent();
        }

        //DELETE api/regulations/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _regulationService.Delete(id, GetSchoolId(), GetUsername(), GetName());
            if (!success)
            {
                return NotFound(new { message = error });
            }
            return NoContent();
        }
    }
}

