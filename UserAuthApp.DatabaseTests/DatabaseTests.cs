using Xunit;
using Microsoft.EntityFrameworkCore;
using UserAuthApp.Api.Data;
using UserAuthApp.Api.Models;
using Testcontainers.MsSql;
using Microsoft.Data.SqlClient;

namespace UserAuthApp.DatabaseTests
{
    public class DatabaseTests : IAsyncLifetime
    {
        private readonly MsSqlContainer _sqlContainer;
        private string _connectionString;

        public DatabaseTests()
        {
            _sqlContainer = new MsSqlBuilder()
                .WithPassword("YourStrong@Passw0rd")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _sqlContainer.StartAsync();
            _connectionString = _sqlContainer.GetConnectionString();
        }

        public async Task DisposeAsync()
        {
            await _sqlContainer.DisposeAsync();
        }

        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        [Fact]
        public async Task Database_ShouldCreateAndSaveUser()
        {
            // Arrange
            await using var context = CreateContext();
            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                PasswordHash = "hashedpassword",
                Age = 25
            };

            // Act
            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Assert
            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
            Assert.NotNull(savedUser);
            Assert.Equal("Test", savedUser.FirstName);
        }

        [Fact]
        public async Task Database_ShouldEnforceDuplicateEmailConstraint()
        {
            // Arrange
            await using var context = CreateContext();
            var user1 = new User
            {
                FirstName = "Test1",
                LastName = "User1",
                Email = "duplicate@example.com",
                PasswordHash = "hashedpassword1",
                Age = 25
            };

            var user2 = new User
            {
                FirstName = "Test2",
                LastName = "User2",
                Email = "duplicate@example.com",
                PasswordHash = "hashedpassword2",
                Age = 30
            };

            // Act & Assert
            context.Users.Add(user1);
            await context.SaveChangesAsync();

            context.Users.Add(user2);
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        }

        [Fact]
        public async Task Database_ShouldPerformUserQuery()
        {
            // Arrange
            await using var context = CreateContext();
            var users = new[]
            {
                new User { FirstName = "John", LastName = "Doe", Email = "john@example.com", PasswordHash = "hash1", Age = 25 },
                new User { FirstName = "Jane", LastName = "Doe", Email = "jane@example.com", PasswordHash = "hash2", Age = 30 }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();

            // Act
            var result = await context.Users
                .Where(u => u.Age >= 25)
                .OrderBy(u => u.FirstName)
                .ToListAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Jane", result[0].FirstName);
            Assert.Equal("John", result[1].FirstName);
        }

        [Fact]
        public async Task Database_ShouldHandleConcurrentAccess()
        {
            // Arrange
            await using var context1 = CreateContext();
            await using var context2 = CreateContext();

            var user = new User
            {
                FirstName = "Test",
                LastName = "User",
                Email = "concurrent@example.com",
                PasswordHash = "hash",
                Age = 25
            };

            context1.Users.Add(user);
            await context1.SaveChangesAsync();

            // Act
            var user1 = await context1.Users.FirstAsync(u => u.Email == "concurrent@example.com");
            var user2 = await context2.Users.FirstAsync(u => u.Email == "concurrent@example.com");

            user1.FirstName = "Updated1";
            user2.FirstName = "Updated2";

            await context1.SaveChangesAsync();

            // Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => context2.SaveChangesAsync());
        }
    }
} 