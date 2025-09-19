using MongoDB.Driver;
using ToursApi.Store.Model;

namespace ToursApi.Store
{
    public class AppDb
    {
        public IMongoCollection<Tour> Tours { get; }
        public IMongoCollection<Position> Positions { get; }

        public AppDb(IMongoDatabase db)
        {
            Tours = db.GetCollection<Tour>("tours");
            Positions = db.GetCollection<Position>("positions");
        }
    }
}
