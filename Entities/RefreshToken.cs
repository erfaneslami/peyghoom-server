using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Peyghoom.Entities;

public class RefreshToken
{
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("create_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("user_id")]
    public ObjectId UserId { get; set; }

    [BsonElement("token")]
    public required string Token { get; set; }

    [BsonElement("expire_at")]
    public DateTime ExpireAt { get; set; }

    [BsonElement("is_revoked")]
    public bool IsRevoked { get; set; } = false;
}