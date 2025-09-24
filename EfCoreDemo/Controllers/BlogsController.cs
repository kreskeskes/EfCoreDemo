using Domain.Models;
using Infrastructure.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace EfCoreDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BlogsController(IBlogService blogService) : ControllerBase
    {
        [HttpPost("{id:int}/explicit-load")]
        public async Task<IActionResult> ExplicitLoad(
        int id,
        [FromQuery] bool loadPosts = true,
        [FromQuery] bool loadOwner = false,
        CancellationToken ct = default)
        {
            var blog = await blogService.LoadNavigationsExplicitlyAsync(id, loadPosts, loadOwner, ct);
            return blog is null ? NotFound() : Ok(blog);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Blog body, CancellationToken ct)
        {
            var created = await blogService.CreateAsync(body, ct);
            return Ok(created);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await blogService.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }

}