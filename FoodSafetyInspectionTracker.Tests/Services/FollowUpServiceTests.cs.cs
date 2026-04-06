using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Models;
using FoodSafetyInspectionTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FoodSafetyInspectionTracker.Tests.Services
{
    public class FollowUpServiceTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAsync_ShouldThrow_WhenClosedWithoutClosedDate()
        {
            using var context = CreateContext(nameof(CreateAsync_ShouldThrow_WhenClosedWithoutClosedDate));

            var premise = new Premise
            {
                Name = "Test Premise",
                Address = "123 Street",
                Town = "Dublin",
                RiskRating = "High"
            };

            context.Premises.Add(premise);
            await context.SaveChangesAsync();

            var inspection = new Inspection
            {
                PremiseId = premise.Id,
                InspectionDate = new DateTime(2026, 4, 1),
                Score = 50,
                Outcome = "Fail",
                Notes = "Test"
            };

            context.Inspections.Add(inspection);
            await context.SaveChangesAsync();

            var service = new FollowUpService(context, NullLogger<FollowUpService>.Instance);

            var followUp = new FollowUp
            {
                InspectionId = inspection.Id,
                DueDate = new DateTime(2026, 4, 5),
                Status = "Closed",
                ClosedDate = null
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.CreateAsync(followUp));
            Assert.Equal("A closed follow-up must have a ClosedDate.", ex.Message);
        }

        [Fact]
        public async Task CloseAsync_ShouldSetStatusToClosed_AndClosedDate()
        {
            using var context = CreateContext(nameof(CloseAsync_ShouldSetStatusToClosed_AndClosedDate));

            var premise = new Premise
            {
                Name = "Test Premise",
                Address = "123 Street",
                Town = "Cork",
                RiskRating = "Low"
            };

            context.Premises.Add(premise);
            await context.SaveChangesAsync();

            var inspection = new Inspection
            {
                PremiseId = premise.Id,
                InspectionDate = DateTime.UtcNow.AddDays(-10),
                Score = 40,
                Outcome = "Fail",
                Notes = "Inspection"
            };

            context.Inspections.Add(inspection);
            await context.SaveChangesAsync();

            var followUp = new FollowUp
            {
                InspectionId = inspection.Id,
                DueDate = DateTime.UtcNow.AddDays(-5),
                Status = "Open"
            };

            context.FollowUps.Add(followUp);
            await context.SaveChangesAsync();

            var service = new FollowUpService(context, NullLogger<FollowUpService>.Instance);

            await service.CloseAsync(followUp.Id);

            var saved = await context.FollowUps.FindAsync(followUp.Id);
            Assert.NotNull(saved);
            Assert.Equal("Closed", saved!.Status);
            Assert.NotNull(saved.ClosedDate);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnOverdueOpenFollowUp_InResults()
        {
            using var context = CreateContext(nameof(GetAllAsync_ShouldReturnOverdueOpenFollowUp_InResults));

            var premise = new Premise
            {
                Name = "Test Premise",
                Address = "123 Street",
                Town = "Galway",
                RiskRating = "Medium"
            };

            context.Premises.Add(premise);
            await context.SaveChangesAsync();

            var inspection = new Inspection
            {
                PremiseId = premise.Id,
                InspectionDate = DateTime.UtcNow.AddDays(-20),
                Score = 45,
                Outcome = "Fail",
                Notes = "Inspection"
            };

            context.Inspections.Add(inspection);
            await context.SaveChangesAsync();

            var overdueFollowUp = new FollowUp
            {
                InspectionId = inspection.Id,
                DueDate = DateTime.UtcNow.AddDays(-10),
                Status = "Open"
            };

            context.FollowUps.Add(overdueFollowUp);
            await context.SaveChangesAsync();

            var service = new FollowUpService(context, NullLogger<FollowUpService>.Instance);

            var result = await service.GetAllAsync();


            Assert.Single(result);
            Assert.Equal("Open", result[0].Status);
            Assert.True(result[0].DueDate < DateTime.UtcNow);
        }
    }
}