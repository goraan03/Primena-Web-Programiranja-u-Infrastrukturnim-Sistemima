using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendAPI.Data.Entities
{
    [Table("Expenses")]
    public class Expense
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string Category { get; set; } // TRANSPORT, ACCOMMODATION, FOOD, TICKETS, SHOPPING, OTHER

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public int TravelId { get; set; }

        // Navigation properties
        [ForeignKey("TravelId")]
        public virtual Travel Travel { get; set; }
    }
}