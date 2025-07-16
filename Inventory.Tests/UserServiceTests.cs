using Inventory.Application.DTOs;
using Inventory.Application.Services;
using Inventory.Core.Entities;
using Inventory.Core.Interfaces;
using FluentAssertions;
using Moq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Inventory.Tests
{
    public class UserServiceTests
    {
        [Fact]
        public async Task ChangePasswordAsync_WithCorrectOldPassword_UpdatesPassword()
        {
            // Arrange
            var userId = 1;
            var oldPassword = "old123";
            var newPassword = "new456";

            CreatePasswordHash(oldPassword, out var oldHash, out var oldSalt);

            var existingUser = new User
            {
                Id = userId,
                Username = "test@example.com",
                PasswordHash = oldHash,
                PasswordSalt = oldSalt
            };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(existingUser);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var mockConfig = new Mock<IConfiguration>();
            var service = new UserService(mockRepo.Object, mockConfig.Object);

            // Act
            var result = await service.ChangePasswordAsync(userId, oldPassword, newPassword);

            // Assert
            result.Should().BeTrue();
            existingUser.PasswordHash.Should().NotEqual(oldHash);
            mockRepo.Verify(r => r.UpdateAsync(existingUser), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var password = "Secret123";
            CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            var user = new User
            {
                Username = "testuser",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User"
            };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByUsernameAsync("testuser")).ReturnsAsync(user);

            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("this_is_a_mocked_unit_test_key_that_is_long_enough_for_HmacSha512_signature_1234567890"); // test keys

            var service = new UserService(mockRepo.Object, mockConfig.Object);

            // Act
            var result = await service.LoginAsync(new UserDto { Username = "testuser", Password = password });

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task RegisterAsync_ShouldFail_IfUsernameAlreadyExists()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.UserExistsAsync("existinguser")).ReturnsAsync(true);

            var mockConfig = new Mock<IConfiguration>();
            var service = new UserService(mockRepo.Object, mockConfig.Object);

            // Act
            var result = await service.RegisterAsync(new UserDto { Username = "existinguser", Password = "pass" });

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Username already exists.");
        }

        [Fact]
        public async Task RegisterAsync_ShouldSucceed_WhenUserIsNew()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.UserExistsAsync("newuser")).ReturnsAsync(false);
            mockRepo.Setup(r => r.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var mockConfig = new Mock<IConfiguration>();
            var service = new UserService(mockRepo.Object, mockConfig.Object);

            // Act
            var result = await service.RegisterAsync(new UserDto { Username = "newuser", Password = "pass" });

            // Assert
            result.Success.Should().BeTrue();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
