using BlazorApp_Final.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Security.Claims;

namespace TestsProject
{
    public class AuthenticationTests
    {
        [Fact]
        public async Task GetAuthenticationStateAsync_UserIsAuthenticated_ReturnsAuthenticatedState()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "testuser" };
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

            var authenticationStateProvider = new ServerAuthenticationStateProvider();
            authenticationStateProvider.SetAuthenticationState(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "testuser") }, "mock")))));

            // Act
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();

            // Assert
            Assert.NotNull(authState.User);
            Assert.True(authState.User.Identity.IsAuthenticated);
            Assert.Equal("testuser", authState.User.Identity.Name);
        }

        [Fact]
        public void PasswordValidation_PasswordMeetsRequirements_ReturnsTrue()
        {
            // Arrange
            var password = "Password1!";
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            // Act
            var result = passwordValidator.ValidateAsync(userManagerMock.Object, null, password).Result;

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void PasswordValidation_PasswordFailsWithoutSpecialCharacter_ReturnsFalse()
        {
            // Arrange
            var password = "Password1";
            var passwordValidator = new PasswordValidator<ApplicationUser>();
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            // Act
            var result = passwordValidator.ValidateAsync(userManagerMock.Object, null, password).Result;

            // Assert
            Assert.False(result.Succeeded);
        }
    }

}
