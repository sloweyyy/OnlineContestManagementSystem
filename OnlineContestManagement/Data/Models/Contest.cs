using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OnlineContestManagement.Models;

namespace OnlineContestManagement.Data.Models
{
  public class Contest
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MinimumParticipant { get; set; }
    public int MaximumParticipant { get; set; }
    public List<PrizeModel> Prizes { get; set; }
    public List<string> ParticipantInformationRequirements { get; set; }
    public string CreatorUserId { get; set; }

  }
}