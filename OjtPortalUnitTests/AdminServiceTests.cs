using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using OjtPortal.Dtos;
using OjtPortal.Entities;
using OjtPortal.Enums;
using OjtPortal.Infrastructure;
using OjtPortal.Repositories;
using OjtPortal.Services;
using System.Net;

namespace OjtPortalUnitTests
{
    public class AdminServiceTests
    {
        private AdminService _adminService;
        private Mock<IAdminRepo> _mockAdminRepo;
        private Mock<IMapper> _mockMapper;
        private Mock<IUserService> _mockUserService;
        private Mock<ILogger<AdminService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockAdminRepo = new Mock<IAdminRepo>();
            _mockMapper = new Mock<IMapper>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger<AdminService>>();
            _adminService = new AdminService(_mockUserService.Object, _mockMapper.Object, _mockAdminRepo.Object, _mockLogger.Object);
        }

        #region CreateAdminAsync_NewAdmin_ShouldReturnExistingUserDto
        [Test]
        public async Task CreateAdminAsync_NewAdmin_ShouldReturnExistingUserDto()
        {
            // Arrange
            var adminId = 1;
            var fakeUser = new UserDto();

            var fakePassword = It.IsAny<string>();

            var fakeAdmin = new CreatedUserDto
            {
                User = new User
                {
                    Id = adminId,
                    Email = fakeUser.Email,
                    FirstName = fakeUser.FirstName,
                    LastName = fakeUser.LastName
                },
                IsPasswordGenerated = true,
                Password = fakePassword
            };

            _mockUserService.Setup(u => u.CreateUserAsync(fakeUser, string.Empty, UserType.Admin))
                .ReturnsAsync((fakeAdmin, null));

            _mockMapper.Setup(m => m.Map<ExistingUserDto>(fakeAdmin.User))
                .Returns(new ExistingUserDto { Id = adminId, Email = fakeUser.Email});

            // Act
            var (result, error) = await _adminService.CreateAdminAsync(fakeUser);

            // Assert
            Assert.NotNull(result, "Result should not be null.");
            Assert.IsNull(error, "Error should be null.");
            Assert.That(result.Id, Is.EqualTo(adminId), "User ID should match.");
            Assert.IsInstanceOf<ExistingUserDto>(result);

            Console.WriteLine($"Test passed for new admin with ID: {result.Id}");

            _mockUserService.Verify(u => u.SendActivationEmailAsync(fakeUser.Email, fakeAdmin.User, fakePassword), Times.Once);
            Console.WriteLine("SendActivationEmailAsync was called as expected.");

            _mockUserService.Verify(u => u.CreateUserAsync(fakeUser, string.Empty, UserType.Admin), Times.Once);
            Console.WriteLine("CreateUserAsync was called as expected.");
        }
        #endregion

        #region GetAdminByIdAsync_AdminIdNotFound_ShouldReturnError
        [Test]
        public async Task GetAdminByIdAsync_AdminIdNotFound_ShouldReturnError()
        {
            // Arrange
            var adminId = It.IsAny<int>();
            Console.WriteLine($"Testing for Id {adminId}: GetAdminByIdAsync_AdminIdNotFound_ShouldReturnError");

            // Act
            var (result, error) = await _adminService.GetAdminByIdAsync(adminId);

            // Assert
            Assert.Null(result);
            Assert.NotNull(error);
            Assert.That(error.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Console.WriteLine("Test finished for GetAdminByIdAsync_ShouldReturnError_WhenAdminNotFound");
        }
        #endregion

        #region GetAdminByIdAsync_AdminIdFound_ShouldReturnAdminDto
        [Test]
        public async Task GetAdminByIdAsync_AdminIdFound_ShouldReturnAdminDto()
        {
            // Arrange
            var adminId = 1;
            Console.WriteLine($"Testing for Id {adminId}: GetAdminByIdAsync_AdminIdFound_ShouldReturnAdminDto");

            var fakeAdmin = new Admin { Id = adminId };
            var fakeAdminDto = new ExistingUserDto { Id = adminId };

            _mockAdminRepo.Setup(r => r.GetAdminByIdAsync(adminId)).ReturnsAsync(fakeAdmin);
            _mockMapper.Setup(m => m.Map<ExistingUserDto>(fakeAdmin)).Returns(fakeAdminDto);

            // Act
            var (result, error) = await _adminService.GetAdminByIdAsync(adminId);

            // Assert
            Assert.NotNull(result, "Result must not be null");
            Assert.Null(error, "Error must be null");
            Assert.IsInstanceOf<ExistingUserDto>(result);
            Assert.That(fakeAdminDto.Id, Is.EqualTo(result.Id), "User ID should mathc");
            Console.WriteLine("Test finished for GetAdminByIdAsync_AdminIdFound_ShouldReturnAdminDto");
        }
        #endregion
    }
}