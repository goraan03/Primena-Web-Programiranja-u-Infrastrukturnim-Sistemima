using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TravelService.Data.Entities
{
    [Table("ChecklistItems")]
    public class ChecklistItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }
        public bool IsCompleted { get; set; }
        [Required]
        public int TravelId { get; set; }
    }
}