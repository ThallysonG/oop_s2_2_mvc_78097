using FoodSafetyInspectionTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodSafetyInspectionTracker.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("SeedData");

            await context.Database.EnsureCreatedAsync();

            string[] roles = { "Admin", "Inspector", "Viewer" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    logger.LogInformation("Role created: {Role}", role);
                }
            }

            await CreateUserAsync(userManager, "admin@test.com", "Admin123!", "Admin", logger);
            await CreateUserAsync(userManager, "inspector@test.com", "Inspector123!", "Inspector", logger);
            await CreateUserAsync(userManager, "viewer@test.com", "Viewer123!", "Viewer", logger);

            if (await context.Premises.AnyAsync())
            {
                logger.LogInformation("Seed skipped because Premises already exist.");
                return;
            }

            var premisesList = new List<Premise>
            {
                new() { Name = "Green Spoon Cafe", Address = "12 Main Street", Town = "Dublin", RiskRating = "Low" },
                new() { Name = "Fresh Bite Bistro", Address = "20 River Road", Town = "Dublin", RiskRating = "Medium" },
                new() { Name = "Ocean Grill", Address = "3 Harbour View", Town = "Dublin", RiskRating = "High" },
                new() { Name = "Sunrise Bakery", Address = "8 Market Lane", Town = "Cork", RiskRating = "Low" },
                new() { Name = "Golden Fork", Address = "14 Grand Parade", Town = "Cork", RiskRating = "Medium" },
                new() { Name = "Harvest Kitchen", Address = "22 Station Road", Town = "Cork", RiskRating = "High" },
                new() { Name = "Urban Tastes", Address = "5 King Street", Town = "Galway", RiskRating = "Low" },
                new() { Name = "Family Table", Address = "9 Bridge Avenue", Town = "Galway", RiskRating = "Medium" },
                new() { Name = "Royal Diner", Address = "17 Castle Way", Town = "Galway", RiskRating = "High" },
                new() { Name = "Spice Route", Address = "31 Church Road", Town = "Dublin", RiskRating = "Medium" },
                new() { Name = "Daily Dish", Address = "42 Mill Street", Town = "Cork", RiskRating = "Low" },
                new() { Name = "Lakeside Meals", Address = "27 Park Lane", Town = "Galway", RiskRating = "High" }
            };

            context.Premises.AddRange(premisesList);
            await context.SaveChangesAsync();

            // Busca novamente os Premises já salvos no banco
            var savedPremises = await context.Premises.ToListAsync();

            var rnd = new Random(42);
            var inspections = new List<Inspection>();

            for (int i = 0; i < 25; i++)
            {
                var premise = savedPremises[rnd.Next(savedPremises.Count)];
                var score = rnd.Next(40, 101);
                var outcome = score >= 60 ? "Pass" : "Fail";

                inspections.Add(new Inspection
                {
                    PremiseId = premise.Id,
                    InspectionDate = DateTime.UtcNow.Date.AddDays(-rnd.Next(0, 90)),
                    Score = score,
                    Outcome = outcome,
                    Notes = $"Inspection note #{i + 1}"
                });
            }

            context.Inspections.AddRange(inspections);
            await context.SaveChangesAsync();

            // Busca novamente as inspeções já salvas
            var savedInspections = await context.Inspections.ToListAsync();

            var followUps = new List<FollowUp>();

            for (int i = 0; i < 10; i++)
            {
                var inspection = savedInspections[rnd.Next(savedInspections.Count)];
                var isClosed = i % 3 == 0;

                var dueDate = inspection.InspectionDate.AddDays(rnd.Next(3, 20));

                followUps.Add(new FollowUp
                {
                    InspectionId = inspection.Id,
                    DueDate = dueDate,
                    Status = isClosed ? "Closed" : "Open",
                    ClosedDate = isClosed ? dueDate.AddDays(rnd.Next(1, 5)) : null
                });
            }

            context.FollowUps.AddRange(followUps);
            await context.SaveChangesAsync();

            logger.LogInformation("SeedData completed successfully.");
        }

        private static async Task CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            string role,
            ILogger logger)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null) return;

            user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (createResult.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                logger.LogInformation("User created: {Email} with role {Role}", email, role);
            }
            else
            {
                logger.LogError("Failed to create user: {Email}. Errors: {Errors}",
                    email, string.Join(", ", createResult.Errors.Select(e => e.Description)));
            }
        }
    }
}