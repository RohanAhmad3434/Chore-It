using System.Threading.Tasks;
using BCrypt.Net;                                   // For password hashing (use full name below)
using Microsoft.EntityFrameworkCore;                // For EF Core methods like SingleOrDefaultAsync
using Parent_Child;
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

        //public async Task<User?> LoginAsync(string email, string password)
        //{
        //    var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
        //    if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        //        return null;

        //    return user;
        //}
        public async Task<UserDto?> LoginAsync(string email, string password)
        {
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
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
