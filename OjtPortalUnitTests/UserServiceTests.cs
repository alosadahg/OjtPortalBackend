using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Repositories;
using OjtPortal.Services;
using System.Net;

namespace OjtPortalUnitTests
{
    public class UserServiceTests
    {
        private Mock<IMapper> _mockMapper;
        private Mock<ILogger<UserService>> _mockLogger;
        private Mock<IUserRepo> _mockUserRepository;
        private Mock<LinkGenerator> _mockLinkGenerator;
        private Mock<IEmailSender> _mockEmailSender;
        private Mock<UserManager<User>> _mockUserManager;
        private Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private Mock<SignInManager<User>> _mockSignInManager;
        private Mock<IOtpRepo> _mockOtpRepo;
        private UserService _userService;

        #region UserService Dependency Setup
        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockLogger = new Mock<ILogger<UserService>>();
            _mockUserRepository = new Mock<IUserRepo>();
            _mockLinkGenerator = new Mock<LinkGenerator>();
            _mockEmailSender = new Mock<IEmailSender>();

            var mockUserStore = new Mock<IUserStore<User>>();
            _mockUserManager = new Mock<UserManager<User>>(
                mockUserStore.Object,
                null, // IOptions<IdentityOptions>
                null, // IPasswordHasher<User>
                null, // IEnumerable<IUserValidator<User>>
                null, // IEnumerable<IPasswordValidator<User>>
                null, // ILookupNormalizer
                null, // IdentityErrorDescriber
                null, // IServiceProvider
                null  // ILogger<UserManager<User>>
            );

            _mockHttpContextAccessor = new Mock<IHttpContextAccessor> { CallBase = true };

            var mockClaimsPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                mockClaimsPrincipalFactory.Object,
                null, // IOptions<IdentityOptions>
                null, // ILogger<SignInManager<User>>
                null, // IAuthenticationSchemeProvider
                null  // IUserConfirmation<User>
            );

            _mockOtpRepo = new Mock<IOtpRepo>();

            _userService = new UserService(
                _mockMapper.Object,
                _mockLogger.Object,
                _mockUserRepository.Object,
                _mockLinkGenerator.Object,
                _mockEmailSender.Object,
                _mockUserManager.Object,
                _mockHttpContextAccessor.Object,
                _mockSignInManager.Object,
                _mockOtpRepo.Object
            );
        }
        #endregion

        #region ChangeDefaultPasswordAsync_ValidPassword_ShouldReturnSuccess
        [Test]
        public async Task ChangeDefaultPasswordAsync_ValidPassword_ShouldReturnSuccess()
        {
            // Arrange
            var changePasswordDto = new ChangeDefaultPasswordDto
            {
                Id = 1,
                NewPassword = "password123",
                ConfirmPassword = "password123"
            };
            var fakeUser = new User
            {
                Id = 1,
                AccountStatus = AccountStatus.PendingPasswordChange
            };
            var fakeToken = "fakeToken";

            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(changePasswordDto.Id))
                .ReturnsAsync((fakeUser, null));
            _mockUserManager.Setup(um => um.GeneratePasswordResetTokenAsync(fakeUser))
                .ReturnsAsync(fakeToken);
            _mockUserManager.Setup(um => um.ResetPasswordAsync(fakeUser, fakeToken, changePasswordDto.NewPassword))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserRepository.Setup(ur => ur.ActivateAccount(fakeUser)).ReturnsAsync(fakeUser);

            // Act
            var (result, error) = await _userService.ChangeDefaultPasswordAsync(changePasswordDto);

            // Assert
            Assert.NotNull(result);
            Assert.Null(error);
            Assert.AreEqual("Successfully changed password", result);
        }
        #endregion

        #region ChangeDefaultPasswordAsync_InvalidPassword_ShouldReturnError
        [Test]
        public async Task ChangeDefaultPasswordAsync_InvalidPassword_ShouldReturnError()
        {
            // Arrange
            var changePasswordDto = new ChangeDefaultPasswordDto
            {
                Id = 1,
                NewPassword = "password123",
                ConfirmPassword = "password456"
            };
            var fakeUser = new User
            {
                Id = 1,
                AccountStatus = AccountStatus.PendingPasswordChange
            };

            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(changePasswordDto.Id))
                .ReturnsAsync((fakeUser, null));

            // Act
            var (result, error) = await _userService.ChangeDefaultPasswordAsync(changePasswordDto);

            // Assert
            Assert.Null(result);
            Assert.NotNull(error);
            Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.First().Code!, Is.EqualTo("Password mismatch"));
        }
        #endregion

        #region ChangeDefaultPasswordAsync_AccountAlreadyActivated_ShouldReturnError
        [Test]
        public async Task ChangeDefaultPasswordAsync_AccountAlreadyActivated_ShouldReturnError()
        {
            // Arrange
            var changePasswordDto = new ChangeDefaultPasswordDto
            {
                Id = 1,
                NewPassword = "password123",
                ConfirmPassword = "password123"
            };
            var fakeUser = new User
            {
                Id = 1,
                AccountStatus = AccountStatus.Active
            };

            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(changePasswordDto.Id))
                .ReturnsAsync((fakeUser, null));

            // Act
            var (result, error) = await _userService.ChangeDefaultPasswordAsync(changePasswordDto);

            // Assert
            Assert.Null(result);
            Assert.NotNull(error);
            Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.First().Code!, Is.EqualTo("Active Account"));
        }
        #endregion

        #region ChangeDefaultPasswordAsync_AccountDeactivated_ShouldReturnError
        [Test]
        public async Task ChangeDefaultPasswordAsync_AccountDeactivated_ShouldReturnError()
        {
            // Arrange
            var changePasswordDto = new ChangeDefaultPasswordDto
            {
                Id = 1,
                NewPassword = "password123",
                ConfirmPassword = "password123"
            };
            var fakeUser = new User
            {
                Id = 1,
                AccountStatus = AccountStatus.Deactivated
            };

            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(changePasswordDto.Id))
                .ReturnsAsync((fakeUser, null));

            // Act
            var (result, error) = await _userService.ChangeDefaultPasswordAsync(changePasswordDto);

            // Assert
            Assert.Null(result);
            Assert.NotNull(error);
            Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(error.Errors.First().Code!, Is.EqualTo("Deactivated Account"));
        }
        #endregion

        #region ChangeDefaultPasswordAsync_AccountNotPendingPasswordChange_ShouldReturnError
        [Test]
        public async Task ChangeDefaultPasswordAsync_AccountNotPendingPasswordChange_ShouldReturnError()
        {
            // Arrange
            var changePasswordDto = new ChangeDefaultPasswordDto
            {
                Id = 1,
                NewPassword = "password123",
                ConfirmPassword = "password123"
            };
            var fakeUser = new User
            {
                Id = 1,
                AccountStatus = AccountStatus.Pending 
            };

            _mockUserRepository.Setup(ur => ur.GetUserByIdAsync(changePasswordDto.Id))
                .ReturnsAsync((fakeUser, null));

            // Act
            var (result, error) = await _userService.ChangeDefaultPasswordAsync(changePasswordDto);

            // Assert
            Assert.Null(result);
            Assert.NotNull(error);
            Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.MethodNotAllowed));
            Assert.That(error.Errors.First().Code!, Is.EqualTo("Not Allowed"));
        }
        #endregion
    }
}