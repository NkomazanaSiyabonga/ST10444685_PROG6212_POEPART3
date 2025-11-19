using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using ProgrammingPOE.Data;
using ProgrammingPOE.Models;

namespace ProgrammingPOE.Services
{
    public class DatabaseAuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public DatabaseAuthService(ApplicationDbContext context)
        {
            _context = context;
        }

        public User Register(string firstName, string lastName, string email, string password, string role)
        {
            // Check if user already exists
            if (_context.Users.Any(u => u.Email.ToLower() == email.ToLower()))
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
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password)
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public User Login(string email, string password, string role)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                throw new Exception("Invalid email or password.");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password.");
            }

            return user;
        }

        public User GetUserById(string userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId);
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.OrderBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();
        }

        public User CreateUser(string firstName, string lastName, string email, string role)
        {
            if (_context.Users.Any(u => u.Email.ToLower() == email.ToLower()))
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
                CreatedAt = DateTime.Now,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!")
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return user;
        }

        public void UpdateUser(string userId, string firstName, string lastName, string email, string role)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.Role = role;
                _context.SaveChanges();
            }
        }

        public void DeleteUser(string userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
        }
    }
}