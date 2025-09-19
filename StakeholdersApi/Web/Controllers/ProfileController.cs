using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using StakeholdersApi.Store;
using StakeholdersApi.Store.Models;
using StakeholdersApi.Web.Dto;
using System.Security.Claims;

namespace StakeholdersApi.Web.Controllers;

[ApiController]
[Route("me/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly AppDb _db;
    public ProfileController(AppDb db) => _db = db;

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!;

    [HttpGet]
    public async Task<ActionResult<ProfileResponse>> Get()
    {
        var p = await _db.Users.Find(u => u.Id == UserId).Project(u => u.Profile).FirstOrDefaultAsync();
        if (p is null) return NotFound();
        return Ok(new ProfileResponse { FirstName = p.FirstName, LastName = p.LastName, AvatarUrl = p.AvatarUrl, Bio = p.Bio, Motto = p.Motto });
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] ProfileUpdateRequest req)
    {
        var update = Builders<User>.Update
          .Set(u => u.Profile.FirstName, req.FirstName)
          .Set(u => u.Profile.LastName, req.LastName)
          .Set(u => u.Profile.AvatarUrl, req.AvatarUrl)
          .Set(u => u.Profile.Bio, req.Bio)
          .Set(u => u.Profile.Motto, req.Motto);
        var result = await _db.Users.UpdateOneAsync(u => u.Id == UserId, update);
        if (result.MatchedCount == 0) return NotFound();
        return NoContent();
    }
}
