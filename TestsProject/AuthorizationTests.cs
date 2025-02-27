using System.Security.Claims;
using System.Threading.Tasks;
using BlazorApp_Final.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace TestsProject
{
    public class AuthorizationTests
    {
        [Fact]
        public async Task UserHasAdminRole_ReturnsTrue()
        {
            // Arrange
            var user = new ApplicationUser { UserName = "adminuser" };
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
            userManagerMock.Setup(um => um.IsInRoleAsync(user, "Admin")).ReturnsAsync(true);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, user.UserName)
            }, "mock"));

            var authorizationServiceMock = new Mock<IAuthorizationService>();
            authorizationServiceMock.Setup(auth => auth.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), null, "Admin"))
                .ReturnsAsync(AuthorizationResult.Success());

            // Act
            var result = await authorizationServiceMock.Object.AuthorizeAsync(claimsPrincipal, null, "Admin");

            // Assert
            Assert.True(result.Succeeded);
        }
    }
}

