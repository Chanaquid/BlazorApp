using Xunit;
using Microsoft.AspNetCore.Identity;
using BlazorApp_Final.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Moq;
using BlazorApp_Final.Models;
using Microsoft.Extensions.Logging; // Add this using directive
using Microsoft.Extensions.Options; // Add this using directive

namespace BlazorApp_Final.Test
{
    public class AuthorizationTests
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthorizationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            _userManager = new UserManager<ApplicationUser>(
                Mock.Of<IUserStore<ApplicationUser>>(),
                Mock.Of<IOptions<IdentityOptions>>(),
                Mock.Of<IPasswordHasher<ApplicationUser>>(),
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new ILookupNormalizer[0],
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<IServiceProvider>(),
                Mock.Of<ILogger<UserManager<ApplicationUser>>>());

            _roleManager = new RoleManager<IdentityRole>(
                Mock.Of<IRoleStore<IdentityRole>>(),
                new IRoleValidator<IdentityRole>[0],
                Mock.Of<ILookupNormalizer>(),
                Mock.Of<IdentityErrorDescriber>(),
                Mock.Of<ILogger<RoleManager<IdentityRole>>>());
        }

        [Fact]
        public async Task Test_User_Is_Admin()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "adminuser" };
            await _userManager.CreateAsync(user);
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
            await _userManager.AddToRoleAsync(user, "Admin");

            // Act
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // Assert
            Assert.True(isAdmin);
        }

        [Fact]
        public async Task Test_User_Is_Not_Admin()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "regularuser" };
            await _userManager.CreateAsync(user);

            // Act
            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            // Assert
            Assert.False(isAdmin);
        }
    }
}
