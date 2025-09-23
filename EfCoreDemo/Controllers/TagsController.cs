using Domain.Models;
using Infrastructure.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfCoreDemo.Controllers
{
    namespace Api.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class TagsController(ITagService tagService) : ControllerBase
        {
            [HttpGet]
            public async Task<ActionResult<List<Tag>>> GetAll(CancellationToken ct = default)
            {
                var tags = await tagService.GetAll();
                return Ok(tags);
            }

            [HttpGet("{id:int}")]
            public async Task<ActionResult<Tag>> GetById(int id, CancellationToken ct = default)
            {
                var tag = await tagService.GetById(id);
                return tag is null ? NotFound() : Ok(tag);
            }

            [HttpPut("{id:int}")]
            public async Task<ActionResult<Tag>> Update(int id, [FromBody] Tag body, CancellationToken ct = default)
            {
                if (body is null) return BadRequest("Tag body is required.");

                var updated = await tagService.Update(body, id);
                return Ok(updated);
            }

            [HttpPost]
            public async Task<ActionResult<Tag>> Create([FromBody] Tag body, CancellationToken ct = default)
            {
                if (body is null) return BadRequest("Tag body is required.");

                var created = await tagService.CreateAsync(body, ct);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }

            [HttpDelete("{id:int}")]
            public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
            {
                var ok = await tagService.DeleteAsync(id, ct);
                return ok ? NoContent() : NotFound();
            }
        }
    }
}