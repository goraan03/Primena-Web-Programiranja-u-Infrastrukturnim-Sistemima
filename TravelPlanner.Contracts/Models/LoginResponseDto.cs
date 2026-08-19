using System.Runtime.Serialization;

namespace TravelPlanner.Contracts.Models
{
    [DataContract]
    public class LoginResponseDto
    {
        [DataMember]
        public string Token { get; set; }

        [DataMember]
        public UserDto User { get; set; }
    }
}