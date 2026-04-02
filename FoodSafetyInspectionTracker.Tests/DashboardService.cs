using FoodSafetyInspectionTracker.Data;
using Microsoft.Extensions.Logging.Abstractions;

namespace FoodSafetyInspectionTracker.Tests
{
    internal class DashboardService
    {
        private ApplicationDbContext context;
        private NullLogger instance;

        public DashboardService(ApplicationDbContext context, NullLogger instance)
        {
            this.context = context;
            this.instance = instance;
        }

        internal async Task GetDashboardAsync(object value1, object value2)
        {
            throw new NotImplementedException();
        }
    }
}