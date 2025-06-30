using Microsoft.EntityFrameworkCore;                // For EF Core methods like SingleOrDefaultAsync
using Parent_Child;
using Parent_Child.Models;                          // For the User model
using BCrypt.Net;                                   // For password hashing (use full name below)
using System.Threading.Tasks;

namespace Parent_Child.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> LoginAsync(string email, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<User> RegisterAsync(User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
