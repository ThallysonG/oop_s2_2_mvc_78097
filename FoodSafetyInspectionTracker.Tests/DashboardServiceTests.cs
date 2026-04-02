using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Enums;
using FoodSafetyInspectionTracker.Models;
using FoodSafetyInspectionTracker.Services;
using FoodSafetyInspectionTracker.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;


namespace FoodSafetyInspectionTracker.Tests
{
    public class DashboardServiceTests
    {
        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            var dublinPremises = new Premises
            {
                Id = 1,
                Name = "Dublin Cafe",
                Address = "1 Main Street",
                Town = "Dublin",
                RiskRating = RiskRating.High
            };

            var corkPremises = new Premises
            {
                Id = 2,
                Name = "Cork Bistro",
                Address = "2 River Lane",
                Town = "Cork",
                RiskRating = RiskRating.Medium
            };

            var inspection = new Inspection
            {
                Id = 1,
                PremisesId = 1,
                InspectionDate = DateTime.Today.AddDays(-10),
                Score = 45,
                Outcome = InspectionOutcome.Fail,
                Notes = "Test inspection"
            };

            var followUp = new FollowUp
            {
                Id = 1,
                InspectionId = 1,
                DueDate = DateTime.Today.AddDays(-2),
                Status = FollowUpStatus.Open
            };

            context.Premises.AddRange(dublinPremises, corkPremises);
            context.Inspections.Add(inspection);
            context.FollowUps.Add(followUp);
            context.SaveChanges();

            return context;
        }

        [Fact]
        public async Task Overdue_FollowUps_Query_Returns_Correct_Count()
        {
            var context = CreateContext();

            var service = new DashboardService(context, NullLogger.Instance);
            var result = await service.GetDashboardAsync(null, null);

            Assert.True(result.OpenOverdueFollowUps >= 1);
        }

        [Fact]
        public async Task Dashboard_Filter_By_Town_Works()
        {
            var context = CreateContext();
            var service = new DashboardService(context, NullLogger.Instance);

            var result = await service.GetDashboardAsync("Dublin", null);

            Assert.NotEmpty(result.FilteredPremises);
            Assert.All(result.FilteredPremises, p => Assert.Equal("Dublin", p.Town));
        }
    }
}