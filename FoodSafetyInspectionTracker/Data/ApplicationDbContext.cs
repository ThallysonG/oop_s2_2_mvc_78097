using FoodSafetyInspectionTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Premise> Premises => Set<Premise>();
        public DbSet<Inspection> Inspections => Set<Inspection>();
        public DbSet<FollowUp> FollowUps => Set<FollowUp>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Premise>()
                .HasMany(p => p.Inspections)
                .WithOne(i => i.Premise)
                .HasForeignKey(i => i.PremiseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inspection>()
                .HasMany(i => i.FollowUps)
                .WithOne(f => f.Inspection)
                .HasForeignKey(f => f.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}