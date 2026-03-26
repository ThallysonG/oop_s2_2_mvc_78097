using FoodSafetyInspectionTracker.Controllers;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace FoodSafetyInspectionTracker.Tests
{
    public class AuthorizationTests
    {
        [Fact]
        public void InspectionsController_Has_Authorize_Attribute()
        {
            var attrs = typeof(InspectionsController).GetCustomAttributes(typeof(AuthorizeAttribute), true);
            Assert.NotEmpty(attrs);
        }

        [Fact]
        public void DashboardController_Has_Authorize_Attribute()
        {
            var attrs = typeof(DashboardController).GetCustomAttributes(typeof(AuthorizeAttribute), true);
            Assert.NotEmpty(attrs);
        }
    }
}