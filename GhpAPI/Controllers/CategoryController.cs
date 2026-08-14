using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GhpAPI.Controllers
{
    [Route("api/categories")]
    [ApiController]
    [Tags("大項管理")]
    public class CategoryController : BaseController
    {
        private readonly CategoryService _categoriesService;

        public CategoryController(AppDbContext db, HistoryService historyService, CategoryService categoriesService)
            : base(db, historyService)
        {
            _categoriesService = categoriesService;
        }

        // GET api/categories
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _categoriesService.GetAll(GetSchoolId());
            return Ok(categories);
        }

        // GET api/categories/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoriesService.GetById(id, GetSchoolId());

            if (result == null)
            {
                return NotFound(new { message = "大項不存在" });
            }

            return Ok(result);
        }

        //POST api/categories
        [HttpPost]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Create([FromBody] SaveCategoryDto dto)
        {
            var (success, error, id) = await _categoriesService.Create(dto, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                return Conflict(new { message = error });
            }

            return CreatedAtAction(nameof(GetById), new { id }, new {  id });
        }

        //PUT api/categories/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "學校管理員")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveCategoryDto dto)
        {
            var (success, error) = await _categoriesService.Update(id, dto, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                return NotFound(new { message = error });
            }

            return NoContent();
        }

        //DELETE api/categories/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "學校管理員")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _categoriesService.Delete(id, GetSchoolId(), GetUsername(), GetName());
            if (!success)
            {
                return NotFound(new { message = error });
            }
            return NoContent();
        }
    }
}
