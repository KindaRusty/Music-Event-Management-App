using Microsoft.AspNetCore.Identity;

namespace MusicEventManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<MusicEvent> CreatedEvents { get; set; } = new List<MusicEvent>();
        public virtual ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
        public virtual UserPreference? UserPreference { get; set; }
    }
}