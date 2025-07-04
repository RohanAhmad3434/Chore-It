using System.Threading.Tasks;
using BCrypt.Net;                                   // For password hashing (use full name below)
using Microsoft.EntityFrameworkCore;                // For EF Core methods like SingleOrDefaultAsync
using Parent_Child.DTOs;
using Parent_Child.Models;                          // For the User model

namespace Parent_Child.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password is required.");

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return new UserDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<User> RegisterAsync(User user)
        {
            // ✅ Validate required fields
            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("Full name is required.");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("Email is required.");

            if (string.IsNullOrWhiteSpace(user.PasswordHash))
                throw new ArgumentException("Password is required.");

            if (string.IsNullOrWhiteSpace(user.Role))
                throw new ArgumentException("Role is required.");

            // ✅ Check if email already exists
            var emailExists = await _context.Users.AnyAsync(u => u.Email == user.Email.Trim());
            if (emailExists)
                throw new Exception($"A user with email {user.Email} already exists.");

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
