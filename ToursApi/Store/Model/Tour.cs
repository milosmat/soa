using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ToursApi.Store.Model
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TourStatus
    {
        draft,
        published,
        archived
    }
    public class Duration
    {
        public int? walk { get; set; }
        public int? bike { get; set; }
        public int? car { get; set; }
    }

    public class TourPoint
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public double lat { get; set; }
        public double lng { get; set; }
        public string name { get; set; } = default!;
        public string? description { get; set; }
        public string? imageUrl { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }

    public class Tour
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string authorId { get; set; } = default!;
        public string title { get; set; } = default!;
        public string description { get; set; } = default!;
        public string difficulty { get; set; } = "easy";   // easy | medium | hard (npr.)
        public string[] tags { get; set; } = Array.Empty<string>();
        public decimal price { get; set; } = 0m;           // (10) start at 0
        public TourStatus status { get; set; } = TourStatus.draft;
        public double lengthKm { get; set; } = 0.0;        // recalculated when points change
        public Duration durations { get; set; } = new();
        public List<TourPoint> points { get; set; } = new();
        public DateTime createdAt { get; set; } = DateTime.UtcNow;

        public bool IsReserved { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
    }
}
