using ProgrammingPOE.Services;
using Xunit;

namespace ProgrammingPOE.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authService = new AuthService();
        }

        [Fact]
        public void Register_NewUser_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@university.com";
            var password = "Password123";
            var role = "Lecturer";

            // Act
            var result = _authService.Register(firstName, lastName, email, password, role);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Lecturer", result.Role);
        }

        [Fact]
        public void Register_DuplicateEmail_ShouldThrowException()
        {
            // Arrange
            var email = "duplicate@test.com";
            _authService.Register("User1", "Test1", email, "pass123", "Lecturer");

            // Act & Assert
            var exception = Assert.Throws<Exception>(() =>
                _authService.Register("User2", "Test2", email, "pass456", "Coordinator"));

            Assert.Contains("already exists", exception.Message);
        }

        [Fact]
        public void Login_ValidCredentials_ShouldReturnUser()
        {
            // Arrange
            var email = "valid.user@test.com";
            var password = "correctPassword";
            var role = "Lecturer";

            _authService.Register("Valid", "User", email, password, role);

            // Act
            var result = _authService.Login(email, password, role);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }
    }
}