using System;
using System.Runtime.Serialization;

namespace TravelPlanner.Contracts.Models
{
    [DataContract]
    public class ExpenseDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Category { get; set; } // TRANSPORT, ACCOMMODATION, FOOD, TICKETS, SHOPPING, OTHER

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public int TravelId { get; set; }
    }
}