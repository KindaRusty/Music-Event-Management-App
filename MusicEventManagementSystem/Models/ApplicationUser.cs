using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace MusicEventManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<MusicEvent> CreatedEvents { get; set; } = new List<MusicEvent>();
        public virtual ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
        public virtual UserPreference? UserPreference { get; set; }
    }
}