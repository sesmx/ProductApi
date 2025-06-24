using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductApi.Api.Controllers;
using ProductApi.Infrastructure.Identity;

namespace ProductApi.Api.Tests.Controllers;

public class AuthControllerTests
{
	private readonly Mock<SignInManager<ApplicationUser>> _mockSignInManager;
	private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
	private readonly Mock<IJwtTokenGenerator> _mockJwt;
	private readonly AuthController _controller;

	public AuthControllerTests()
	{
		// Setup mocks for UserManager and SignInManager
		var store = new Mock<IUserStore<ApplicationUser>>();
		_mockUserManager = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

		var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
		var userClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
		_mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
			_mockUserManager.Object,
			contextAccessor.Object,
			userClaimsPrincipalFactory.Object,
			null, null, null, null);

		_mockJwt = new Mock<IJwtTokenGenerator>();

		_controller = new AuthController(_mockSignInManager.Object, _mockUserManager.Object, _mockJwt.Object);
	}

	[Fact]
	public async Task Login_UserNotFound_ReturnsBadRequest()
	{
		// Arrange
		var loginDto = new LoginRequestDto { Username = "nonexistent", Password = "password" };
		_mockUserManager.Setup(x => x.FindByNameAsync(loginDto.Username))
			.ReturnsAsync((ApplicationUser)null);

		// Act
		var result = await _controller.Login(new Infrastructure.Data.Configurations.LoginRequestDto(loginDto.Username, loginDto.Password));

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Invalid credentials.", badRequestResult.Value);
	}

	[Fact]
	public async Task Login_InvalidPassword_ReturnsBadRequest()
	{
		// Arrange
		var loginDto = new LoginRequestDto { Username = "user", Password = "wrongpassword" };
		var user = new ApplicationUser { UserName = loginDto.Username };
		_mockUserManager.Setup(x => x.FindByNameAsync(loginDto.Username))
			.ReturnsAsync(user);

		// Set up sign-in result to indicate failure.
		var signInResult = Microsoft.AspNetCore.Identity.SignInResult.Failed;
		_mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
			.ReturnsAsync(signInResult);

		// Act
		var result = await _controller.Login(new Infrastructure.Data.Configurations.LoginRequestDto(loginDto.Username, loginDto.Password));

		// Assert
		var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
		Assert.Equal("Invalid credentials.", badRequestResult.Value);
	}

	[Fact]
	public async Task Login_ValidCredentials_ReturnsOkWithToken()
	{
		// Arrange
		var loginDto = new LoginRequestDto { Username = "admin", Password = "Dxfactor@123" };
		var user = new ApplicationUser { UserName = loginDto.Username };
		var tokenResponse = new TokenResponseDto { Token = "valid.token.value" };

		_mockUserManager.Setup(x => x.FindByNameAsync(loginDto.Username))
			.ReturnsAsync(user);

		var successResult = Microsoft.AspNetCore.Identity.SignInResult.Success;
		_mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
			.ReturnsAsync(successResult);

		_mockJwt.Setup(x => x.Generate(user))
			.Returns(new Infrastructure.Data.Configurations.TokenResponseDto(tokenResponse.Token, DateTime.UtcNow.AddMinutes(120)));

		// Act
		var result = await _controller.Login(new Infrastructure.Data.Configurations.LoginRequestDto(loginDto.Username, loginDto.Password));

		// Assert
		var okResult = Assert.IsType<OkObjectResult>(result);
		Assert.Equal(tokenResponse.Token, ((Infrastructure.Data.Configurations.TokenResponseDto)okResult.Value).AccessToken);
	}

	[Fact]
	public void Logout_ReturnsNoContent()
	{
		// Act
		var result = _controller.Logout();

		// Assert
		Assert.IsType<NoContentResult>(result);
	}
}

public class LoginRequestDto
{
	public string Username { get; set; }
	public string Password { get; set; }
}

public class TokenResponseDto
{
	public string Token { get; set; }
}