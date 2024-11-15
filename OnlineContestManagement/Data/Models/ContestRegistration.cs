using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace OnlineContestManagement.Data.Models
{
    public class ContestRegistration
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ContestId { get; set; }

        
        public string UserId { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string Email { get; set; }

        public Dictionary<string, string> AdditionalInfo { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string Status { get; set; }

       
    }
}
