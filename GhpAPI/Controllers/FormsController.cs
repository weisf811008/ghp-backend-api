using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/forms")]
    [ApiController]
   
    [Tags("表單管理")]

    public class FormsController : BaseController
    {
        private readonly FormService _formService;
        public FormsController(AppDbContext db, HistoryService historyService, FormService formService)
       : base(db, historyService)
        {
            _formService = formService;
        }

        //GET api/forms
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetAll()
        {
            var result = await _formService.GetAll(GetSchoolId());

            return Ok(result);
        }

        //GET api/forms/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetById(int id)
        {
            var result = await _formService.GetById(id, GetSchoolId());

            if (result == null)
            { 
                return NotFound(new { message = "表單不存在" });
            }

            return Ok(result);
        }

        //POST api/forms
        [HttpPost]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Create([FromBody] SaveFormDto dto)
        {
            var (success, error, id) = await _formService.Create(dto, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                return Conflict(new { message = error });
            }

            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        //PUT api/forms/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "學校管理員")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveFormDto dto)
        {
            var (success, error) = await _formService.Update(id, dto, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                if (error == "表單不存在")
                {
                    return NotFound(new { message = error });
                }

                return Conflict(new { message = error });
            }

            return NoContent();
        }

        //DELETE api/forms/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _formService.Delete(id, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                return NotFound(new { message = error });

            }

            return NoContent();
        }
    }
}
