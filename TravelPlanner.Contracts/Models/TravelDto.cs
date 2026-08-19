using System;
using System.Runtime.Serialization;

namespace TravelPlanner.Contracts.Models
{
    [DataContract]
    public class TravelDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public decimal Budget { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public int UserId { get; set; }
    }
}