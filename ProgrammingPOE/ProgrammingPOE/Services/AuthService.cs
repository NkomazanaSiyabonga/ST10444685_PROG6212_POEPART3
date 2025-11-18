namespace ProgrammingPOE.Services
{
    public class AuthService : IAuthService
    {
        private static List<User> _users = new List<User>();
        private static Dictionary<string, string> _passwords = new Dictionary<string, string>();

        public User Register(string firstName, string lastName, string email, string password, string role)
        {
            // Check if user already exists
            if (_users.Any(u => u.Email.ToLower() == email.ToLower()))
            {
                throw new Exception("User with this email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = role,
                CreatedAt = DateTime.Now
            };

            _users.Add(user);
            _passwords[user.Id] = password; // Store password (in real app, hash this)

            return user;
        }

        public User Login(string email, string password, string role)
        {
            var user = _users.FirstOrDefault(u =>
                u.Email.ToLower() == email.ToLower() &&
                u.Role == role);

            if (user != null && _passwords.ContainsKey(user.Id) && _passwords[user.Id] == password)
            {
                return user;
            }

            throw new Exception("Invalid email, password, or role.");
        }

        public User GetUserById(string userId)
        {
            return _users.FirstOrDefault(u => u.Id == userId);
        }

        // Add these methods to AuthService class
        public List<User> GetAllUsers()
        {
            return _users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();
        }

        public User CreateUser(string firstName, string lastName, string email, string role)
        {
            // Check if user already exists
            if (_users.Any(u => u.Email.ToLower() == email.ToLower()))
            {
                throw new Exception("User with this email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = role,
                CreatedAt = DateTime.Now
            };

            _users.Add(user);
            // Default password for new users
            _passwords[user.Id] = "Password123!";

            return user;
        }

        public void UpdateUser(string userId, string firstName, string lastName, string email, string role)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.Role = role;
            }
        }

        public void DeleteUser(string userId)
        {
            var user = _users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                _users.Remove(user);
                _passwords.Remove(userId);
            }
        }
    }
}