using Inventory.Application.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Inventory.Infrastructure.Data;
using Inventory.Infrastructure.Repositories;
using Inventory.Core.Entities;
using System.Text;
using System.Security.Cryptography;
using Inventory.Core.Interfaces;
using Moq;

namespace Inventory.Tests
{
    public class UserServiceTests
    {

        [Fact]
        public void ChangePassword_WithCorrectOldPassword_UpdatesPassword()
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
            mockRepo.Setup(r => r.GetById(userId)).Returns(existingUser);
            mockRepo.Setup(r => r.Update(It.IsAny<User>()));

            var service = new UserService(mockRepo.Object);

            // Act
            var result = service.ChangePassword(userId, oldPassword, newPassword);

            // Assert
            Assert.True(result);
            Assert.NotEqual(oldHash, existingUser.PasswordHash);
            mockRepo.Verify(r => r.Update(existingUser), Times.Once);
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new System.Security.Cryptography.HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var password = "Secret123";
            byte[] passwordHash, passwordSalt;
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            var user = new User
            {
                Username = "testuser",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            using (var context = new AppDbContext(options))
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(options))
            {
                var repo = new UserRepository(context);

                // Act
                var result = await repo.LoginAsync("testuser", password);

                // Assert
                result.Should().Be("test-token-or-service-calls-for-now");
            }
        }
    }
}
