using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OnlineContestManagement.Models
{
    public class Contest
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string Organizer { get; set; }

        
        public int MinParticipants { get; set; }
        public int MaxParticipants { get; set; }

        
        public List<string> Skills { get; set; }

        
        public List<string> ParticipantInfoRequirements { get; set; }

        
        public List<Prize> Prizes { get; set; }
    }

    public class Prize
    {
        public string Name { get; set; }
        public string Image { get; set; } 
        public string Description { get; set; }
        public decimal Value { get; set; } 
        public int Quantity { get; set; } 
    }
}
