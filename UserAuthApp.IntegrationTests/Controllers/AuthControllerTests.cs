using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using UserAuthApp.Api;
using UserAuthApp.Api.Data;
using UserAuthApp.Api.Models.DTOs;
using UserAuthApp.Api.Services;
using Xunit;
using System.Text.Json;
using Moq;
using System.Threading;

namespace UserAuthApp.IntegrationTests.Controllers
{
    public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly string _dbName;

        public AuthControllerTests(WebApplicationFactory<Program> factory)
        {
            _dbName = $"TestDb_{Guid.NewGuid()}";
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Test");
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    var descriptors = services.Where(d => d.ServiceType.Name.Contains("DbContext")).ToList();
                    foreach (var d in descriptors)
                    {
                        services.Remove(d);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase(_dbName);
                    });
                });
            });
        }

        private HttpClient CreateClient()
        {
            return _factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var client = CreateClient();
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "TestPass123!",
                Age = 25
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/register", registerDto);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Registration successful", content);
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_ShouldReturnBadRequest()
        {
            // Arrange
            var client = CreateClient();
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "duplicate@example.com",
                Password = "TestPass123!",
                Age = 25
            };

            // Act
            var response1 = await client.PostAsJsonAsync("/api/Auth/register", registerDto);
            Assert.Equal(HttpStatusCode.OK, response1.StatusCode);

            var response2 = await client.PostAsJsonAsync("/api/Auth/register", registerDto);
            var content = await response2.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response2.StatusCode);
            Assert.Contains("Email already exists", content);
        }

        [Fact]
        public async Task Register_WithInvalidData_ShouldReturnBadRequest()
        {
            // Arrange
            var client = CreateClient();
            var registerDto = new RegisterDto
            {
                FirstName = "",
                LastName = "",
                Email = "invalid-email",
                Password = "short",
                Age = -1
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/register", registerDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturnSuccess()
        {
            // Arrange
            var client = CreateClient();
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "login@example.com",
                Password = "TestPass123!",
                Age = 25
            };
            var registerResponse = await client.PostAsJsonAsync("/api/Auth/register", registerDto);
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginDto = new LoginDto
            {
                Email = "login@example.com",
                Password = "TestPass123!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("Login successful", content);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ShouldReturnBadRequest()
        {
            // Arrange
            var client = CreateClient();
            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "WrongPass123!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);
            var content = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.Contains("Invalid email or password", content);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnUsersList()
        {
            // Arrange
            var client = CreateClient();
            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "getusers@example.com",
                Password = "TestPass123!",
                Age = 25
            };
            var registerResponse = await client.PostAsJsonAsync("/api/Auth/register", registerDto);
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            // Act
            var response = await client.GetAsync("/api/Auth/users");
            var content = await response.Content.ReadAsStringAsync();
            var users = JsonSerializer.Deserialize<List<object>>(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(users);
            Assert.NotEmpty(users);
        }

        [Fact]
        public async Task Register_WhenServiceThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>()))
                          .ThrowsAsync(new Exception("Database connection error"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAuthService>(_ => mockAuthService.Object);
                });
            }).CreateClient();

            var registerDto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "TestPass123!",
                Age = 25
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/register", registerDto);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Login_WhenServiceThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
                          .ThrowsAsync(new Exception("Database connection error"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAuthService>(_ => mockAuthService.Object);
                });
            }).CreateClient();

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "TestPass123!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_WhenServiceThrowsException_ShouldReturnInternalServerError()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.GetAllUsersAsync())
                          .ThrowsAsync(new Exception("Database connection error"));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAuthService>(_ => mockAuthService.Object);
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/api/Auth/users");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Register_WhenOperationTimesOut_ShouldReturnServiceUnavailable()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.RegisterAsync(It.IsAny<RegisterDto>()))
                          .ThrowsAsync(new OperationCanceledException());

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAuthService>(_ => mockAuthService.Object);
                });
            }).CreateClient();

            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "timeout@test.com",
                Password = "Password123!",
                Age = 30
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/register", registerDto);

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }

        [Fact]
        public async Task Login_WhenOperationTimesOut_ShouldReturnServiceUnavailable()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
                          .ThrowsAsync(new OperationCanceledException());

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAuthService>(_ => mockAuthService.Object);
                });
            }).CreateClient();

            var loginDto = new LoginDto
            {
                Email = "timeout@test.com",
                Password = "Password123!"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }

        [Fact]
        public async Task GetAllUsers_WhenOperationTimesOut_ShouldReturnServiceUnavailable()
        {
            // Arrange
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.GetAllUsersAsync())
                          .ThrowsAsync(new OperationCanceledException());

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IAuthService>(_ => mockAuthService.Object);
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync("/api/Auth/users");

            // Assert
            Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        }
    }
}