using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Entities;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GhpAPI.Controllers
{
    [Route("api/visitingForms")]
    [ApiController]
    [Tags("訪視表管理")]
   
    public class VisitingFormController : BaseController
    {
        private readonly VisitingFormService _visitingFormService;
        public VisitingFormController(AppDbContext db, HistoryService historyService, VisitingFormService visitingFormService)
        : base(db, historyService)
        {
            _visitingFormService = visitingFormService;
        }

        //GET api/visitingForms
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetAll()
        {
            var visitingForms = await _visitingFormService.GetAll(GetSchoolId());

            return Ok(visitingForms);
        }

        //GET api/visitingForms/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetById(int id)
        {
           
            var result = await _visitingFormService.GetById(id, GetSchoolId());

            if (result == null)
            {
                return NotFound(new { message = "訪視表不存在" });
            }

            return Ok(result);
        }

        //POST api/visitingForms
        [HttpPost]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Create([FromBody] SaveVisitingFormDto dto)
        {

            var (success, error, id) = await _visitingFormService.Create(dto, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                return Conflict(new { message = error });
            }

            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        //PUT api/visitingForms/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Update(int id, [FromBody] SaveVisitingFormDto dto)
        {
            var (success, error) = await _visitingFormService.Update(id, dto, GetSchoolId(), GetUsername(), GetName());
            if (!success)
            {
                if (error == "訪視表不存在")
                {
                    return NotFound(new { message = error });
                }

                return Conflict(new { message = error });
            }
            return NoContent();
        }

        //DELETE api/visitingForms/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _visitingFormService.Delete(id, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                return NotFound(new { message = error });
            }
            return NoContent();
    }
}
}
