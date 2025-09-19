using MongoDB.Driver;
using PurchaseApi.Store.Model;

namespace PurchaseApi.Store
{
    public class AppDb
    {
        public IMongoCollection<ShoppingCart> Carts { get; }
        public IMongoCollection<TourPurchaseToken> Purchases { get; }

        public AppDb(IMongoDatabase db)
        {
            Carts = db.GetCollection<ShoppingCart>("carts");
            Purchases = db.GetCollection<TourPurchaseToken>("purchases");
        }
    }
}
