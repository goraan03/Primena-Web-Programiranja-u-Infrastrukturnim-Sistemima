using System;
using System.Runtime.Serialization;

namespace TravelPlanner.Contracts.Models
{
    [DataContract]
    public class DestinationDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public DateTime ArrivalDate { get; set; }

        [DataMember]
        public DateTime DepartureDate { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int TravelId { get; set; }
    }
}