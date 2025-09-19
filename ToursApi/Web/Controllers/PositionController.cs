using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ToursApi.Store;
using ToursApi.Store.Model;
using ToursApi.Web.Auth;

namespace ToursApi.Web.Controllers
{
    [ApiController]
    [Route("positions")]
    [Authorize]
    public class PositionsController : ControllerBase
    {
        private readonly AppDb _db;
        public PositionsController(AppDb db) => _db = db;

        public class PositionDto
        {
            public double Lat { get; set; }
            public double Lng { get; set; }
        }

        // POST /positions → snimi trenutnu poziciju korisnika
        [HttpPost]
        public async Task<IActionResult> SetPosition([FromBody] PositionDto req)
        {
            var pos = new Position
            {
                userId = User.UserId(),
                lat = req.Lat,
                lng = req.Lng
            };

            await _db.Positions.InsertOneAsync(pos);
            return NoContent();
        }

        // GET /positions/me → poslednja moja pozicija
        [HttpGet("me")]
        public async Task<ActionResult<Position?>> GetMyPosition()
        {
            var pos = await _db.Positions
                .Find(p => p.userId == User.UserId())
                .SortByDescending(p => p.createdAt)
                .FirstOrDefaultAsync();

            return Ok(pos);
        }
    }
}
