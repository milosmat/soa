using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PurchaseApi.Store.Model
{
    public class TourPurchaseToken
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public string TourId { get; set; } = default!;
        public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    }
}
