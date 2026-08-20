using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Data.Entities
{
    [Table("Activities")]
    public class Activity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(10)]
        public string Time { get; set; }

        [Required]
        [MaxLength(200)]
        public string Location { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal EstimatedCost { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "PLANNED"; // PLANNED, RESERVED, COMPLETED, CANCELLED

        [Required]
        public int TravelId { get; set; }

        // Navigation properties
        [ForeignKey("TravelId")]
        public virtual Travel Travel { get; set; }
    }
}