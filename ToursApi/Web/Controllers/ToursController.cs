using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using ToursApi.Store;
using ToursApi.Store.Model;
using ToursApi.Web.Auth;
using ToursApi.Web.Dto;

namespace ToursApi.Web.Controllers
{
    [ApiController]
    [Route("tours")]
    [Authorize]
    public class ToursController : ControllerBase
    {
        private readonly AppDb _db;
        public ToursController(AppDb db) => _db = db;

        // (10) Kreiraj draft turu (price=0)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTourRequest req)
        {
            var tour = new Tour
            {
                authorId = User.UserId(),
                title = req.title,
                description = req.description,
                difficulty = req.difficulty,
                tags = req.tags ?? Array.Empty<string>(),
                price = 0m,
                status = TourStatus.draft
            };
            await _db.Tours.InsertOneAsync(tour);
            return Created($"/tours/{tour.Id}", new { tour.Id });
        }

        // (10) Autor vidi svoje ture
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<Tour>>> Mine()
        {
            var list = await _db.Tours.Find(t => t.authorId == User.UserId()).ToListAsync();
            return Ok(list);
        }

        // (11) Dodaj ključnu tačku i izračunaj dužinu
        [HttpPost("{id}/points")]
        public async Task<IActionResult> AddPoint(string id, [FromBody] AddPointRequest req)
        {
            var tour = await _db.Tours.Find(t => t.Id == id && t.authorId == User.UserId()).FirstOrDefaultAsync();
            if (tour is null) return NotFound("Tour not found or not yours.");

            tour.points.Add(new TourPoint
            {
                Id = ObjectId.GenerateNewId().ToString(),
                lat = req.lat,
                lng = req.lng,
                name = req.name,
                description = req.description,
                imageUrl = req.imageUrl
            });

            tour.lengthKm = CalcLengthKm(tour.points);

            var update = Builders<Tour>.Update
                .Set(t => t.points, tour.points)
                .Set(t => t.lengthKm, tour.lengthKm);

            await _db.Tours.UpdateOneAsync(t => t.Id == id && t.authorId == tour.authorId, update);
            return NoContent();
        }

        [HttpGet]
        [Authorize] // ili [Authorize] ako samo ulogovani turisti smeju
        public async Task<ActionResult<IEnumerable<Tour>>> All()
        {
            var list = await _db.Tours
                .Find(t => t.status == TourStatus.published)
                .ToListAsync();

            return Ok(list);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateTourRequestDtos req)
        {
            var tour = await _db.Tours.Find(t => t.Id == id && t.authorId == User.UserId()).FirstOrDefaultAsync();
            if (tour is null) return NotFound("Tour not found or not yours.");

            var update = Builders<Tour>.Update
                .Set(t => t.title, req.Title ?? tour.title)
                .Set(t => t.description, req.Description ?? tour.description)
                .Set(t => t.difficulty, req.Difficulty ?? tour.difficulty)
                .Set(t => t.tags, req.Tags ?? tour.tags)
                .Set(t => t.price, req.Price ?? tour.price)
                .Set(t => t.status, req.Status ?? tour.status);

            await _db.Tours.UpdateOneAsync(t => t.Id == id, update);

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetOne(string id)
        {
            var tour = await _db.Tours.Find(t => t.Id == id).FirstOrDefaultAsync();
            if (tour is null) return NotFound();
            return Ok(tour);
        }

        [HttpPost("{id}/reserve")]
        public async Task<IActionResult> ReserveTour(string id)
        {
            var update = Builders<Tour>.Update.Set(t => t.IsReserved, true);
            var result = await _db.Tours.UpdateOneAsync(t => t.Id == id, update);
            if (result.ModifiedCount == 0) return NotFound();
            return Ok();
        }

        [HttpPost("{id}/confirm")]
        public async Task<IActionResult> ConfirmTour(string id)
        {
            var update = Builders<Tour>.Update
                .Set(t => t.IsReserved, false)
                .Set(t => t.IsCancelled, false);
            var result = await _db.Tours.UpdateOneAsync(t => t.Id == id, update);
            if (result.ModifiedCount == 0) return NotFound();
            return Ok();
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelTour(string id)
        {
            var update = Builders<Tour>.Update
                .Set(t => t.IsReserved, false)
                .Set(t => t.IsCancelled, true);
            var result = await _db.Tours.UpdateOneAsync(t => t.Id == id, update);
            if (result.ModifiedCount == 0) return NotFound();
            return Ok();
        }
        private static double CalcLengthKm(List<TourPoint> pts)
        {
            if (pts.Count < 2) return 0.0;
            const double R = 6371.0; // km
            double total = 0.0;
            for (int i = 1; i < pts.Count; i++)
            {
                total += Haversine(pts[i - 1].lat, pts[i - 1].lng, pts[i].lat, pts[i].lng, R);
            }
            return Math.Round(total, 3);
        }
        private static double Haversine(double lat1, double lon1, double lat2, double lon2, double R)
        {
            double ToRad(double d) => d * Math.PI / 180.0;
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }
}
