using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicEventManagementSystem.Models
{
    /// <summary>
    /// Khớp với bảng EventRegistrations trong DB đã cài.txt
    /// </summary>
    public class EventRegistration
    {
        [Key]
        public int RegistrationID { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } = "Unpaid";

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPrice { get; set; }

        // --- Foreign Keys ---

        [Required]
        public int EventID { get; set; }
        [ForeignKey("EventID")]
        public virtual MusicEvent? MusicEvent { get; set; }

        [Required]
        public string UserID { get; set; } = string.Empty;
        [ForeignKey("UserID")]
        public virtual ApplicationUser? ApplicationUser { get; set; }

        [Required]
        public int PricingTierID { get; set; }
        [ForeignKey("PricingTierID")]
        public virtual PricingTier? PricingTier { get; set; }

        // Navigation properties
        public virtual ICollection<RegistrationData> RegistrationData { get; set; } = new List<RegistrationData>();
    }
}