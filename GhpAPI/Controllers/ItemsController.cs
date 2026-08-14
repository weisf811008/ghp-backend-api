using GhpAPI.Data;
using GhpAPI.DTOs;
using GhpAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GhpAPI.Controllers
{
    [Route("api/items")]
    [ApiController]
    [Tags("細項管理")]
   

    public class ItemsController : BaseController
    {
        private readonly ItemService _itemService;
        public ItemsController(AppDbContext db, HistoryService historyService, ItemService itemService)
        : base(db, historyService)
        {
            _itemService = itemService;
        }

        //GET api/items
        [HttpGet]
        [Authorize(Roles = "學校管理員,巡檢人員")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _itemService.GetAll(GetSchoolId());

            return Ok(result);
        }

        //GET api/items/{id}
        [HttpGet("{id}")]
        [Authorize(Roles = "學校管理員,巡檢人員")]

        public async Task<IActionResult> GetById(int id)
        {
            var result = await _itemService.GetById(id, GetSchoolId());

            if (result == null)
            {
                return NotFound(new { message = "細項不存在" });
            }

            return Ok(result);
        }


        //POST  api/items
        [HttpPost]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Create([FromBody] SaveItemDto dto)
        {
            var (success, error, id) = await _itemService.Create(dto, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                return Conflict(new { message = error });
            }

            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        //PUT api/items/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "學校管理員")]

        public async Task<IActionResult> Update(int id, [FromBody] SaveItemDto dto)
        {
            var (success, error) = await _itemService.Update(id,dto, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                if (error == "細項不存在")
                {
                    return NotFound(new { message = error });
                }

                return Conflict(new { message = error });
            }

            return NoContent();
        }

        //Delete api/items/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "學校管理員")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _itemService.Delete(id, GetSchoolId(), GetUsername(), GetName());

            if (!success)
            {
                return NotFound(new { message = error });
            }

            return NoContent();
        }
    }
}
