using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace OnlineContestManagement.Data.Models
{
  public class Payment
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public int OrderId { get; set; }
    public string ProductName { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
    public string CancelUrl { get; set; }
    public string ReturnUrl { get; set; }
    public string PaymentLink { get; set; }
    public string Status { get; set; }
    public string ContestId { get; set; }
    public string UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}