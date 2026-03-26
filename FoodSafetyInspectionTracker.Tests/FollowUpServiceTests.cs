using FoodSafetyInspectionTracker.Data;
using FoodSafetyInspectionTracker.Enums;
using FoodSafetyInspectionTracker.Services;
using FoodSafetyInspectionTracker.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace FoodSafetyInspectionTracker.Tests
{
    public class FollowUpServiceTests
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
        public async Task FollowUp_Cannot_Be_Closed_Without_ClosedDate()
        {
            var context = CreateContext();
            var service = new FollowUpService(context, new NullLogger<FollowUpService>());

            var vm = new FollowUpCreateViewModel
            {
                InspectionId = 2,
                DueDate = new DateTime(2026, 3, 15),
                Status = FollowUpStatus.Closed,
                ClosedDate = null
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateFollowUpAsync(vm, "tester"));
        }

        [Fact]
        public async Task FollowUp_DueDate_Cannot_Be_Before_InspectionDate()
        {
            var context = CreateContext();
            var service = new FollowUpService(context, new NullLogger<FollowUpService>());

            var vm = new FollowUpCreateViewModel
            {
                InspectionId = 2,
                DueDate = new DateTime(2026, 2, 1),
                Status = FollowUpStatus.Open
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.CreateFollowUpAsync(vm, "tester"));
        }
    }
}