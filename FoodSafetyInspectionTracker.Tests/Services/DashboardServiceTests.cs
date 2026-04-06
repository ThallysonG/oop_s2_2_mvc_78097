using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Models;
using FoodSafetyInspectionTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FoodSafetyInspectionTracker.Tests.Services
{
    public class DashboardServiceTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetDashboardAsync_ShouldReturnCorrectCounts()
        {
            using var context = CreateContext(nameof(GetDashboardAsync_ShouldReturnCorrectCounts));

            var premise = new Premise
            {
                Name = "Premise 1",
                Address = "123 Street",
                Town = "Dublin",
                RiskRating = "High"
            };

            context.Premises.Add(premise);
            await context.SaveChangesAsync();

            var inspectionThisMonthFail = new Inspection
            {
                PremiseId = premise.Id,
                InspectionDate = DateTime.UtcNow.AddDays(-2),
                Score = 40,
                Outcome = "Fail",
                Notes = "Failed inspection"
            };

            var inspectionThisMonthPass = new Inspection
            {
                PremiseId = premise.Id,
                InspectionDate = DateTime.UtcNow.AddDays(-1),
                Score = 85,
                Outcome = "Pass",
                Notes = "Passed inspection"
            };

            context.Inspections.AddRange(inspectionThisMonthFail, inspectionThisMonthPass);
            await context.SaveChangesAsync();

            var overdueFollowUp = new FollowUp
            {
                InspectionId = inspectionThisMonthFail.Id,
                DueDate = DateTime.UtcNow.AddDays(-1),
                Status = "Open"
            };

            context.FollowUps.Add(overdueFollowUp);
            await context.SaveChangesAsync();

            var service = new DashboardService(context, NullLogger<DashboardService>.Instance);

            var result = await service.GetDashboardAsync(null, null);

            Assert.Equal(2, result.InspectionsThisMonth);
            Assert.Equal(1, result.FailedInspectionsThisMonth);
            Assert.Equal(1, result.OverdueFollowUps);
        }
    }
}