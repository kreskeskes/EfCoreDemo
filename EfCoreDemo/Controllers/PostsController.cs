using Domain.Models;
using Infrastructure;
using Infrastructure.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController(IPostService postService) : ControllerBase
    {

        [HttpGet("{id:int}/top-comments")]
        public async Task<IActionResult> GetWithTopComments(
            int id,
            [FromQuery] int topN = 3,
            [FromQuery] bool newestFirst = true,
            CancellationToken ct = default)
        {
            if (topN <= 0) return BadRequest("topN must be > 0.");

            var post = await postService.GetWithTopCommentsAsync(id, topN, newestFirst, ct);
            return post is null ? NotFound() : Ok(post);
        }

        [HttpDelete("{id:int}/comments/old/materialize")]
        public async Task<IActionResult> DeleteOldCommentsMaterialize(
            int id,
            [FromQuery] int olderThanDays,
            CancellationToken ct = default)
        {
            if (olderThanDays <= 0) return BadRequest("olderThanDays must be > 0.");

            var affected = await postService.DeleteOldComments_MaterializeAsync(id, olderThanDays, ct);
            return Ok(new { affected });
        }

        [HttpDelete("{id:int}/comments/old/server")]
        public async Task<IActionResult> DeleteOldCommentsServerSide(
            int id,
            [FromQuery] int olderThanDays,
            CancellationToken ct = default)
        {
            if (olderThanDays <= 0) return BadRequest("olderThanDays must be > 0.");

            var affected = await postService.DeleteOldComments_NoMaterializeAsync(id, olderThanDays, ct);
            return Ok(new { affected });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Post body, CancellationToken ct = default)
        {
            if (body is null) return BadRequest("Post body is required.");

            var created = await postService.CreateAsync(body, ct);
            return Created($"/api/posts/{created.Id}", created);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
        {
            var ok = await postService.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }


        [HttpGet("lazy-n-plus-one")]
        public async Task<IActionResult> ListWithLazy(CancellationToken ct)
        {
            var posts = await _postsListSource(ct);

            var payload = posts.Select(p => new { p.Id, p.Title, CommentsCount = p.Comments.Count }).ToList();
            return Ok(payload);
        }

        private async Task<List<Post>> _postsListSource(CancellationToken ct) =>
    await HttpContext.RequestServices
        .GetRequiredService<ApplicationDbContext>()
        .Posts
        .AsNoTracking()
        .OrderByDescending(p => p.Id)
        .Take(10)
        .ToListAsync(ct);
    }
}
