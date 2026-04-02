using FoodSafetyInspectionTracker.Enums;
using FoodSafetyInspectionTracker.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Premises> Premises => Set<Premises>();
        public DbSet<Inspection> Inspections => Set<Inspection>();
        public DbSet<FollowUp> FollowUps => Set<FollowUp>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Premises>()
                .HasMany(p => p.Inspections)
                .WithOne(i => i.Premises)
                .HasForeignKey(i => i.PremisesId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inspection>()
                .HasMany(i => i.FollowUps)
                .WithOne(f => f.Inspection)
                .HasForeignKey(f => f.InspectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data needed by the tests
            builder.Entity<Premises>().HasData(
                new Premises
                {
                    Id = 1,
                    Name = "Central Cafe",
                    Address = "1 Main Street",
                    Town = "Dublin",
                    RiskRating = RiskRating.High
                },
                new Premises
                {
                    Id = 2,
                    Name = "Harbour Diner",
                    Address = "22 Coast Road",
                    Town = "Cork",
                    RiskRating = RiskRating.Medium
                }
            );

            builder.Entity<Inspection>().HasData(
                new Inspection
                {
                    Id = 1,
                    PremisesId = 1,
                    InspectionDate = new DateTime(2026, 3, 10),
                    Score = 55,
                    Outcome = InspectionOutcome.Fail,
                    Notes = "Requires follow-up."
                },
                new Inspection
                {
                    Id = 2,
                    PremisesId = 1,
                    InspectionDate = new DateTime(2026, 3, 10),
                    Score = 72,
                    Outcome = InspectionOutcome.Pass,
                    Notes = "Used by follow-up tests."
                }
            );

            builder.Entity<FollowUp>().HasData(
                new FollowUp
                {
                    Id = 1,
                    InspectionId = 1,
                    DueDate = new DateTime(2026, 3, 15),
                    Status = FollowUpStatus.Open,
                    ClosedDate = null
                }
            );
        }
    }
}