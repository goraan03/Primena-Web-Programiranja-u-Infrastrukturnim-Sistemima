using System.Runtime.Serialization;

namespace TravelPlanner.Contracts.Models
{
    [DataContract]
    public class UserDto
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Role { get; set; } // USER, ADMIN
    }
}