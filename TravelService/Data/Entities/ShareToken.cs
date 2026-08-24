using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelService.Data.Entities
{
    [Table("ShareTokens")]
    public class ShareToken
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(64)]
        public string Token { get; set; }
        [Required]
        public int TravelId { get; set; }
        [Required]
        [MaxLength(10)]
        public string AccessType { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; } = true;
    }
}