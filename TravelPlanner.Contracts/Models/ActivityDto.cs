using System;
using System.Runtime.Serialization;

namespace TravelPlanner.Contracts.Models
{
    [DataContract]
    public class ActivityDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal EstimatedCost { get; set; }

        [DataMember]
        public string Status { get; set; } // PLANNED, RESERVED, COMPLETED, CANCELLED

        [DataMember]
        public int TravelId { get; set; }
    }
}