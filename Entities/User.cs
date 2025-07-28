using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Peyghoom.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; set; }

    [BsonElement("create_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("user_name")]
    public string UserName { get; set; } = null!;

    [BsonElement("first_name")]
    public string FirstName { get; set; } = null!;

    [BsonElement("last_name")]
    public string LastName { get; set; } = null!;

    [BsonElement("phone_number")]
    public long PhoneNumber { get; set; }

    [BsonElement("conversions")]
    public List<Conversation> Conversions { get; set; } = new List<Conversation>();
}

public class Conversation
{
    [BsonElement("create_at")] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [BsonElement("messages")]
    public List<Message> Messages { get; set; } = new List<Message>();

    [BsonElement("participant_user_id")]
    public int ParticipantUserId { get; set; }
}

public class Message
{
    [BsonElement("create_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("sender_user_id")]
    public int SenderUserId { get; set; }

    [BsonElement("receiver_user_id")]
    public int ReceiverUserId { get; set; }

    [BsonElement("content")]
    public MessageContent? Content { get; set; }
}

public class MessageContent
{
    [BsonElement("text")]
    public string? Text { get; set; }
}
