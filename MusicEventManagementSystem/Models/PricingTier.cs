using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicEventManagementSystem.Models
{
    public class PricingTier
    {
        [Key]
        public int PricingTierID { get; set; }

        [Required]
        [StringLength(100)]
        public string TierName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int? AvailableTickets { get; set; }
        public int SoldTickets { get; set; } = 0;

        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public virtual MusicEvent? MusicEvent { get; set; }

        public virtual ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
    }
}