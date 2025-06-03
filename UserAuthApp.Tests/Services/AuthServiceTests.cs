using Xunit;
using Moq;
using UserAuthApp.Api.Services;
using UserAuthApp.Api.Data;
using UserAuthApp.Api.Models;
using UserAuthApp.Api.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace UserAuthApp.Tests.Services
{
    public class AuthServiceTests
    {
        private static IQueryable<T> CreateAsyncQueryable<T>(IEnumerable<T> items)
        {
            return new TestAsyncEnumerable<T>(items);
        }

        private class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            private readonly IEnumerable<T> _enumerable;

            public TestAsyncEnumerable(IEnumerable<T> enumerable)
                : base(enumerable)
            {
                _enumerable = enumerable;
            }

            public TestAsyncEnumerable(Expression expression)
                : base(expression)
            {
                _enumerable = this;
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                throw new OperationCanceledException();
            }

            IQueryProvider IQueryable.Provider
            {
                get { return new TestAsyncQueryProvider<T>(this); }
            }
        }

        private class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public T Current
            {
                get { return _inner.Current; }
            }

            public ValueTask<bool> MoveNextAsync()
            {
                throw new OperationCanceledException();
            }

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return new ValueTask();
            }
        }

        private class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            public TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner ?? throw new ArgumentNullException(nameof(inner));
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object? Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                throw new OperationCanceledException();
            }

            public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
            {
                return new TestAsyncEnumerable<TResult>(expression);
            }
        }

        [Fact]
        public async Task RegisterAsync_WhenOperationTimesOut_ShouldThrowTimeoutException()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            var mockSet = new Mock<DbSet<User>>();

            var users = new List<User>().AsQueryable();
            var asyncUsers = CreateAsyncQueryable(users);

            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(asyncUsers.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(asyncUsers.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(asyncUsers.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(asyncUsers.GetEnumerator());
            mockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Callback(() => throw new OperationCanceledException());

            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(c => c.Set<User>()).Returns(mockSet.Object);

            var service = new AuthService(mockContext.Object);
            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "Password123!",
                Age = 30
            };

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                service.RegisterAsync(registerDto));
        }

        [Fact]
        public async Task LoginAsync_WhenOperationTimesOut_ShouldThrowTimeoutException()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            var mockSet = new Mock<DbSet<User>>();

            var users = new List<User>().AsQueryable();
            var asyncUsers = CreateAsyncQueryable(users);

            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(asyncUsers.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(asyncUsers.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(asyncUsers.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(asyncUsers.GetEnumerator());
            mockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Callback(() => throw new OperationCanceledException());

            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(c => c.Set<User>()).Returns(mockSet.Object);

            var service = new AuthService(mockContext.Object);
            var loginDto = new LoginDto
            {
                Email = "john@example.com",
                Password = "Password123!"
            };

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                service.LoginAsync(loginDto));
        }

        [Fact]
        public async Task GetAllUsersAsync_WhenOperationTimesOut_ShouldThrowTimeoutException()
        {
            // Arrange
            var mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());
            var mockSet = new Mock<DbSet<User>>();

            var users = new List<User>().AsQueryable();
            var asyncUsers = CreateAsyncQueryable(users);

            mockSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(asyncUsers.Provider);
            mockSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(asyncUsers.Expression);
            mockSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(asyncUsers.ElementType);
            mockSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(asyncUsers.GetEnumerator());
            mockSet.As<IAsyncEnumerable<User>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Callback(() => throw new OperationCanceledException());

            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(c => c.Set<User>()).Returns(mockSet.Object);

            var service = new AuthService(mockContext.Object);

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                service.GetAllUsersAsync());
        }
    }
}