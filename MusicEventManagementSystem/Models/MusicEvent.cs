using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicEventManagementSystem.Models
{
    public class MusicEvent
    {
        [Key]
        public int EventID { get; set; }

        [Required(ErrorMessage = "Tên sự kiện là bắt buộc")]
        [StringLength(200, MinimumLength = 5)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Required]
        [StringLength(300)]
        public string Location { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Genre { get; set; }

        [Column(TypeName = "TEXT")]
        public string? Description { get; set; }

        public int? MaxAttendees { get; set; }

        public int CurrentAttendees { get; set; } = 0;

        public DateTime? RegistrationDeadline { get; set; }

        [Column(TypeName = "TEXT")]
        public string? ImageUrl { get; set; }

        [Required]
        public string CreatedByUserID { get; set; } = string.Empty;

        [ForeignKey("CreatedByUserID")]
        public virtual ApplicationUser? CreatedByUser { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastModifiedDate { get; set; }

        public bool IsPublished { get; set; } = false;

        public virtual ICollection<RequiredField> RequiredFields { get; set; } = new List<RequiredField>();
        public virtual ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
        public virtual ICollection<PricingTier> PricingTiers { get; set; } = new List<PricingTier>();
    }
}