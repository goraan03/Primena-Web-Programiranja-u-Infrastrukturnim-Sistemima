namespace TravelPlanner.Contracts.Models
{
    public class ShareTokenDto
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public int TravelId { get; set; }
        public string AccessType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateShareTokenDto
    {
        public int TravelId { get; set; }
        public string AccessType { get; set; } = "VIEW";
        public int ExpiresInDays { get; set; } = 7;
    }
}