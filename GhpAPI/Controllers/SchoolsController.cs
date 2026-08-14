using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/admin/schools")]
    [ApiController]
    [Tags("系統管理-學校")]
    [Authorize(Roles = "系統管理員")]
    public class SchoolsController : BaseController
    {
        private readonly SchoolService _schoolService;

        public SchoolsController(AppDbContext db, HistoryService historyService, SchoolService schoolService)
            : base(db, historyService)
        {
            _schoolService = schoolService;
        }

        // GET api/admin/schools
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _schoolService.GetAll();
            return Ok(result);
        }

        // GET api/admin/schools/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _schoolService.GetById(id);
            if (result == null) return NotFound(new { message = "學校不存在" });
            return Ok(result);
        }

        // POST api/admin/schools
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSchoolDto dto)
        {
            var (success, error, id) = await _schoolService.Create(dto, GetUsername(), GetName());
            if (!success) return Conflict(new { message = error });
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        // PUT api/admin/schools/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSchoolDto dto)
        {
            var (success, error) = await _schoolService.Update(id, dto, GetUsername(), GetName());
            if (!success) return NotFound(new { message = error });
            return NoContent();
        }

        // DELETE api/admin/schools/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _schoolService.Delete(id, GetUsername(), GetName());
            if (!success) return NotFound(new { message = error });
            return NoContent();
        }
    }
}