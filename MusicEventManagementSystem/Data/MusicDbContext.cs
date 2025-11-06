using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MusicEventManagementSystem.Models;

namespace MusicEventManagementSystem.Data
{
    public class MusicDbContext : IdentityDbContext<ApplicationUser>
    {
        public MusicDbContext(DbContextOptions<MusicDbContext> options) : base(options)
        {
        }

        // Add DbSet properties for each entity
        public DbSet<MusicEvent> MusicEvents { get; set; }
        public DbSet<PricingTier> PricingTiers { get; set; }
        public DbSet<RequiredField> RequiredFields { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }
        public DbSet<RegistrationData> RegistrationData { get; set; }
        public DbSet<UserPreference> UserPreferences { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 1-n relationship: ApplicationUser -> MusicEvent
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.CreatedEvents)
                .WithOne(e => e.CreatedByUser)
                .HasForeignKey(e => e.CreatedByUserID)
                .OnDelete(DeleteBehavior.Restrict); 

            // Configure 1-to-many relationship: ApplicationUser -> EventRegistration
            builder.Entity<ApplicationUser>()
                .HasMany(u => u.Registrations)
                .WithOne(r => r.ApplicationUser)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Cascade); // Delete registrations when the user is deleted

            // Configure 1-to-1 relationship: ApplicationUser <-> UserPreference
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.UserPreference)
                .WithOne(p => p.ApplicationUser)
                .HasForeignKey<UserPreference>(p => p.UserID);

            // Configure 1-to-many relationship: MusicEvent -> PricingTier
            builder.Entity<MusicEvent>()
                .HasMany(e => e.PricingTiers)
                .WithOne(p => p.MusicEvent)
                .HasForeignKey(p => p.EventID)
                .OnDelete(DeleteBehavior.Cascade); // Delete pricing tiers when the event is deleted

            // Configure 1-to-many relationship: MusicEvent -> RequiredField
            builder.Entity<MusicEvent>()
                .HasMany(e => e.RequiredFields)
                .WithOne(r => r.MusicEvent)
                .HasForeignKey(r => r.EventID)
                .OnDelete(DeleteBehavior.Cascade); // Delete required fields when the event is deleted

            // Configure 1-to-many relationship: MusicEvent -> EventRegistration
            builder.Entity<MusicEvent>()
                .HasMany(e => e.Registrations)
                .WithOne(r => r.MusicEvent)
                .HasForeignKey(r => r.EventID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting an Event if registrations exist

            // Configure 1-to-many relationship: PricingTier -> EventRegistration
            builder.Entity<PricingTier>()
                .HasMany(p => p.Registrations)
                .WithOne(r => r.PricingTier)
                .HasForeignKey(r => r.PricingTierID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a pricing tier if registrations exist

            // Configure 1-to-many relationship: EventRegistration -> RegistrationData
            builder.Entity<EventRegistration>()
                .HasMany(r => r.RegistrationData)
                .WithOne(d => d.EventRegistration)
                .HasForeignKey(d => d.RegistrationID)
                .OnDelete(DeleteBehavior.Cascade); // Delete registration data when the registration is deleted

            // Configure 1-to-many relationship: RequiredField -> RegistrationData
            builder.Entity<RequiredField>()
                .HasMany(rf => rf.RegistrationData)
                .WithOne(d => d.RequiredField)
                .HasForeignKey(d => d.FieldID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a field if it has registration data
        }
    }
}