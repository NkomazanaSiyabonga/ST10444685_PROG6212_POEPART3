namespace ProgrammingPOE.Services
{
    public interface IAuthService
    {
        User Register(string firstName, string lastName, string email, string password, string role);
        User Login(string email, string password, string role);
        User GetUserById(string userId);

        // NEW METHODS FOR USER MANAGEMENT
        List<User> GetAllUsers();
        User CreateUser(string firstName, string lastName, string email, string role);
        void UpdateUser(string userId, string firstName, string lastName, string email, string role);
        void DeleteUser(string userId);
    }

    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PasswordHash { get; set; }
    }
}