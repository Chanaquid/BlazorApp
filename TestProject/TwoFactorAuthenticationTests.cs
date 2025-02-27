using System.Threading.Tasks;
using BlazorApp_Final.Components.Account.Pages.Manage;
using BlazorApp_Final.Data;
using Microsoft.AspNetCore.Http;
using BlazorApp_Final.Services; 
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity;
using System.Security.Claims;
using System.Net.Http;

public class TwoFactorAuthenticationTests
{
    public HttpContext HttpContext { get; set; }
    public IdentityRedirectManager RedirectManager { get; set; }
    public IdentityUserAccessor UserAccessor { get; set; }
    public SignInManager<ApplicationUser> SignInManager { get; set; }
    public UserManager<ApplicationUser> UserManager { get; set; }

    private readonly Mock<IdentityRedirectManager> _redirectManagerMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
    private readonly Mock<IdentityUserAccessor> _userAccessorMock;
    private readonly Mock<HttpContext> _httpContextMock;

    public TwoFactorAuthenticationTests()
    {
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _signInManagerMock = new Mock<SignInManager<ApplicationUser>>(
            _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>(), null, null, null, null);
        _userAccessorMock = new Mock<IdentityUserAccessor>();
        _redirectManagerMock = new Mock<IdentityRedirectManager>();
        _httpContextMock = new Mock<HttpContext>();
    }

    [Fact]
    public async Task OnInitializedAsync_SetsPropertiesCorrectly()
    {
        // Arrange
        var user = new ApplicationUser();
        var claimsPrincipal = new ClaimsPrincipal(); // Create a ClaimsPrincipal instance
        //_userAccessorMock.Setup(x => x.GetUserAsync(claimsPrincipal)).ReturnsAsync(user); // Use the ClaimsPrincipal instance
        _userManagerMock.Setup(x => x.GetAuthenticatorKeyAsync(user)).ReturnsAsync("authenticatorKey");
        _userManagerMock.Setup(x => x.GetTwoFactorEnabledAsync(user)).ReturnsAsync(true);
        _signInManagerMock.Setup(x => x.IsTwoFactorClientRememberedAsync(user)).ReturnsAsync(true);
        _userManagerMock.Setup(x => x.CountRecoveryCodesAsync(user)).ReturnsAsync(5);

        var component = new TwoFactorAuthentication
        {
            UserManager = _userManagerMock.Object,
            SignInManager = _signInManagerMock.Object,
            UserAccessor = _userAccessorMock.Object,
            RedirectManager = _redirectManagerMock.Object,
            HttpContext = _httpContextMock.Object
        };

        // Act
        await component.OnInitializedAsync();

        // Assert
        Assert.True(component.hasAuthenticator);
        Assert.True(component.is2faEnabled);
        Assert.True(component.isMachineRemembered);
        Assert.Equal(5, component.recoveryCodesLeft);
    }

    [Fact]
    public async Task OnSubmitForgetBrowserAsync_CallsForgetTwoFactorClientAsync_AndRedirects()
    {
        // Arrange
        var component = new TwoFactorAuthentication
        {
            SignInManager = _signInManagerMock.Object,
            RedirectManager = _redirectManagerMock.Object,
            HttpContext = _httpContextMock.Object
        };

        // Act
        await component.OnSubmitForgetBrowserAsync();

        // Assert
        _signInManagerMock.Verify(x => x.ForgetTwoFactorClientAsync(), Times.Once);
        _redirectManagerMock.Verify(x => x.RedirectToCurrentPageWithStatus(
            "The current browser has been forgotten. When you login again from this browser you will be prompted for your 2fa code.",
            _httpContextMock.Object), Times.Once);
    }
}
