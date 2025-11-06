using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MusicEventManagementSystem.Models
{
    /// <summary>
    /// Khớp với bảng UserPreferences trong DB đã cài.txt
    /// </summary>
    public class UserPreference
    {
        [Key]
        public int PreferenceID { get; set; }

        [Column(TypeName = "TEXT")]
        public string? PreferredGenres { get; set; }

        [Column(TypeName = "TEXT")]
        public string? PreferredLocations { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal? MaxPriceRange { get; set; }

        public bool NotificationEnabled { get; set; } = true;
        public bool EmailReminderEnabled { get; set; } = true;
        public int ReminderHoursBefore { get; set; } = 24;
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        // --- Foreign Key ---
        [Required]
        public string UserID { get; set; } = string.Empty;
        [ForeignKey("UserID")]
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}