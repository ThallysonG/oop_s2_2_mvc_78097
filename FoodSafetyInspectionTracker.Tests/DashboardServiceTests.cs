using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

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
            return context;
        }

        [Fact]
        public async Task Overdue_FollowUps_Query_Returns_Correct_Count()
        {
            var context = CreateContext();

            var service = new DashboardService(context, new NullLogger<DashboardService>());
            var result = await service.GetDashboardAsync(null, null);

            Assert.True(result.OpenOverdueFollowUps >= 1);
        }

        [Fact]
        public async Task Dashboard_Filter_By_Town_Works()
        {
            var context = CreateContext();
            var service = new DashboardService(context, new NullLogger<DashboardService>());

            var result = await service.GetDashboardAsync("Dublin", null);

            Assert.All(result.FilteredPremises, p => Assert.Equal("Dublin", p.Town));
        }
    }
}