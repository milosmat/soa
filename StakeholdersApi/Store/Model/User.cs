using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StakeholdersApi.Store.Models;

public enum Role { TOURIST, GUIDE, ADMIN }

public class Profile
{
    [BsonElement("firstName")] public string? FirstName { get; set; }
    [BsonElement("lastName")] public string? LastName { get; set; }
    [BsonElement("avatarUrl")] public string? AvatarUrl { get; set; }
    [BsonElement("bio")] public string? Bio { get; set; }
    [BsonElement("motto")] public string? Motto { get; set; }
}

public class User
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = default!;
    [BsonElement("username")] public string Username { get; set; } = default!;
    [BsonElement("email")] public string Email { get; set; } = default!;
    [BsonElement("passwordHash")] public string PasswordHash { get; set; } = default!;
    [BsonElement("role"), BsonRepresentation(BsonType.String)] public Role Role { get; set; } = Role.TOURIST;
    [BsonElement("blocked")] public bool Blocked { get; set; } = false;
    [BsonElement("profile")] public Profile Profile { get; set; } = new();
    [BsonElement("createdAt")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
