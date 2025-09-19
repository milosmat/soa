using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PurchaseApi.Store.Model
{
    public class OrderItem
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string TourId { get; set; } = default!;
        public string TourName { get; set; } = default!;
        public decimal Price { get; set; }
    }
}
