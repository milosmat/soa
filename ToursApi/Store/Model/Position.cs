using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ToursApi.Store.Model
{
    public class Position
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;

        public string userId { get; set; } = default!;
        public double lat { get; set; }
        public double lng { get; set; }
        public DateTime createdAt { get; set; } = DateTime.UtcNow;
    }
}
