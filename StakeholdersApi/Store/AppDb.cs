using MongoDB.Driver;
using StakeholdersApi.Store.Models;

namespace StakeholdersApi.Store;
public class AppDb
{
    public IMongoCollection<User> Users { get; }
    public AppDb(IMongoDatabase db) => Users = db.GetCollection<User>("users");
}
