using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq; // Add this using directive
using Xunit; // Add this using directive
using BlazorApp_Final.Models; // Add this using directive

namespace BlazorApp_Final.Test
{
    public class AuthenticationTests
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;

        public AuthenticationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            var context = new ApplicationDbContext(options);

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            _authenticationStateProvider = new ServerAuthenticationStateProvider(_userManagerMock.Object, context);
        }

        [Fact]
        public async Task Test_Authenticated_User()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser" };
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync(user);

            // Act
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            // Assert
            Assert.NotNull(authState.User.Identity);
            Assert.True(authState.User.Identity.IsAuthenticated);
        }

        [Fact]
        public async Task Test_Not_Authenticated_User()
        {
            // Arrange
            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

            // Assert
            Assert.NotNull(authState.User.Identity);
            Assert.False(authState.User.Identity.IsAuthenticated);
        }
    }
}
