using Microsoft.EntityFrameworkCore;
using UserAuthApp.Api.Data;
using UserAuthApp.Api.Models;
using Xunit;

namespace UserAuthApp.Tests.Data
{
    public class DatabaseTests
    {
        private readonly ApplicationDbContext _context;

        public DatabaseTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Database_CanAddAndRetrieveUser()
        {
            // Arrange
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Age = 25
            };

            // Act
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Assert
            var retrievedUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            Assert.NotNull(retrievedUser);
            Assert.Equal(user.FirstName, retrievedUser.FirstName);
            Assert.Equal(user.LastName, retrievedUser.LastName);
        }

        [Fact]
        public async Task Database_CanUpdateUser()
        {
            // Arrange
            var user = new User
            {
                FirstName = "Original",
                LastName = "Name",
                Email = "original@example.com",
                PasswordHash = "hashedpassword",
                Age = 25
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            user.FirstName = "Updated";
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Assert
            var updatedUser = await _context.Users.FindAsync(user.Id);
            Assert.NotNull(updatedUser);
            Assert.Equal("Updated", updatedUser!.FirstName);
        }
    }
} 