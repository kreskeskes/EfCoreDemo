using Domain.Models;
using Infrastructure;
using Infrastructure.ServiceContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EfCoreDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController(IUserService usersService) : ControllerBase
    {
        [HttpPatch("{id:int}/name/tracked")]
        public async Task<IActionResult> UpdateNameTracked(
            int id,
            [FromBody] UpdateNameRequest body,
            CancellationToken ct)
        {
            var user = await usersService.UpdateNameTrackedAsync(id, body.NewName, ct);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpPatch("{id:int}/name/notracking")]
        public async Task<IActionResult> UpdateNameNoTracking(int id, [FromBody] UpdateNameRequest body, CancellationToken ct)
        {
            var user = await usersService.UpdateNameNotTrackedAsync(id, body.NewName, ct);
            return user is null ? NotFound() : Ok(user);
        }

        [HttpGet("reload/{id:int}")]
        public async Task<IActionResult> Reload(int id, CancellationToken ct)
        {
            var user = await usersService.ReloadAsync(id, ct);
            if (user is null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost("transfer-blogs")]
        public async Task<IActionResult> TransferBlogs([FromBody] TransferBlogsRequest body, CancellationToken ct)
        {
            var ok = await usersService.TransferBlogsAsync(body.FromUserId, body.ToUserId, ct);
            return ok ? Ok(new { moved = true }) : NotFound();
        }

        [HttpPost("transfer-blogs-with-savepoint")]
        public async Task<IActionResult> TransferBlogs_WithSavepoint([FromBody] TransferBlogsRequest body, CancellationToken ct)
        {
            var ok = await usersService.TransferBlogsWithSavepointsAsync(body.FromUserId, body.ToUserId, ct);
            return ok ? Ok(new { moved = true }) : NotFound();
        }

        // Showcases the importance of having IsUnique() index against properties in configs
        [HttpPost("demo-race/create-user")]
        public async Task<IActionResult> DemoRace([FromServices] IServiceScopeFactory sf, CancellationToken ct)
        {
            const string email = "race@example.com";

            // cleanup 
            using (var clean = sf.CreateScope())
            {
                var db = clean.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await db.Database.ExecuteSqlInterpolatedAsync($@"DELETE FROM ""Users"" WHERE ""Email"" = {email}", ct);
            }

            var t1 = CreateInNewScopeAsync(sf, new User { Name = "Alice#1", Email = email }, ct);
            var t2 = CreateInNewScopeAsync(sf, new User { Name = "Alice#2", Email = email }, ct);

            var created = await Task.WhenAll(t1, t2);

            using var verify = sf.CreateScope();
            var dbv = verify.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var count = await dbv.Users.AsNoTracking().CountAsync(u => u.Email == email, ct);

            return Ok(new { inserted = created.Select(u => new { u.Id, u.Name, u.Email }), duplicatesWithSameEmail = count });
        }

        static async Task<User> CreateInNewScopeAsync(IServiceScopeFactory sf, User user, CancellationToken ct)
        {
            using var scope = sf.CreateScope();
            var svc = scope.ServiceProvider.GetRequiredService<IUserService>();
            return await svc.CreateAsync(user, ct);
        }

        [HttpGet("getWithBlogsAndPosts")]
        public async Task<IActionResult> GetUserWithBlogsAndPosts([FromQuery] int userId, CancellationToken ct)
        {
            return Ok(await usersService.GetUserWithBlogsAndPosts(userId, ct));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] User body, CancellationToken ct)
        {
            var created = await usersService.CreateAsync(body, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var user = await usersService.GetById(id, ct);
            if (user != null)
                return Ok(user);

            return NotFound();

        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await usersService.DeleteAsync(id, ct);
            return ok ? NoContent() : NotFound();
        }
    }

    public sealed class UpdateNameRequest
    {
        public string NewName { get; set; } = default!;
    }

    public sealed class TransferBlogsRequest
    {
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
    }
}