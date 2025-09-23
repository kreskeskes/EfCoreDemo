using Domain.Models;
using Infrastructure.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfCoreDemo.Controllers
{
    namespace Api.Controllers
    {
        namespace Api.Controllers
        {
            [ApiController]
            [Route("api/[controller]")]
            public class CommentsController(ICommentService commentService) : ControllerBase
            {

                [HttpGet("{id:int}")]
                public async Task<IActionResult> GetById(int id, CancellationToken ct)
                {
                    var comment = await commentService.GetByIdAsync(id, ct);
                    return comment is null ? NotFound() : Ok(comment);
                }

                [HttpPost]
                public async Task<IActionResult> Create([FromBody] Comment body, CancellationToken ct)
                {
                    if (body is null) return BadRequest("Comment body is required.");
                    var created = await commentService.CreateAsync(body, ct);
                    return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
                }

                [HttpDelete("{id:int}")]
                public async Task<IActionResult> Delete(int id, CancellationToken ct)
                {
                    var ok = await commentService.DeleteAsync(id, ct);
                    return ok ? NoContent() : NotFound();
                }

                [HttpGet("search")]
                public async Task<IActionResult> SearchByAuthor([FromQuery] string authorName, CancellationToken ct)
                {
                    if (string.IsNullOrWhiteSpace(authorName))
                        return BadRequest("authorName is required.");

                    IReadOnlyList<Comment> result = await commentService.SearchByAuthorAsync(authorName, ct);
                    return Ok(result);
                }

                [HttpGet("search/raw")]
                public async Task<IActionResult> SearchByAuthorRaw([FromQuery] string authorName, CancellationToken ct)
                {
                    if (string.IsNullOrWhiteSpace(authorName))
                        return BadRequest("authorName is required.");

                    IReadOnlyList<Comment> result = await commentService.RawSearchByCommentAuthorName(authorName, ct);
                    return Ok(result);
                }
            }
        }
    }
}