using System.Runtime.Serialization;

namespace TravelPlanner.Contracts.Models
{
    [DataContract]
    public class LoginRequestDto
    {
        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}