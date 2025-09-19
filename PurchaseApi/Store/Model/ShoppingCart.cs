using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PurchaseApi.Store.Model
{
    public class ShoppingCart
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public List<OrderItem> Items { get; set; } = new();
        public decimal TotalPrice => Items.Sum(i => i.Price);
    }
}
