using MongoDB.Driver;
using StakeholdersApi.Store.Models;

namespace StakeholdersApi.Store;
public static class Seed
{
    public static async Task EnsureAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDb>();

        try
        {
            await db.Users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Username),
            new CreateIndexOptions { Unique = true }));
        }
        catch { }
        try
        {
            await db.Users.Indexes.CreateOneAsync(
            new CreateIndexModel<User>(Builders<User>.IndexKeys.Ascending(u => u.Email),
            new CreateIndexOptions { Unique = true }));
        }
        catch { }

        var admin = await db.Users.Find(u => u.Role == Role.ADMIN).FirstOrDefaultAsync();
        if (admin is null)
        {
            var email = Environment.GetEnvironmentVariable("ADMIN__EMAIL") ?? "admin@local";
            var pass = Environment.GetEnvironmentVariable("ADMIN__PASSWORD") ?? "admin123";
            await db.Users.InsertOneAsync(new User
            {
                Username = "admin",
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(pass),
                Role = Role.ADMIN
            });
            Console.WriteLine($"[seed] Admin: {email} / {pass}");
        }
    }
}
