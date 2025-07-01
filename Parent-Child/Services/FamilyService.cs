using Microsoft.EntityFrameworkCore;
using Parent_Child.Models;
namespace Parent_Child.Services
{
    public class FamilyService : IFamilyService
    {
        private readonly AppDbContext _context;

        public FamilyService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetChildrenAsync(int parentId)
        {
            return await _context.Users
                .Where(u => u.ParentId == parentId)
                .Include(u => u.Tasks)
                .ToListAsync();
        }

        public async Task<User?> AddChildAsync(int parentId, User child)
        {
            // Check email exists
            if (await _context.Users.AnyAsync(u => u.Email == child.Email))
                throw new Exception("Email already exists.");

            child.ParentId = parentId;
            child.Role = "Child";
            child.PasswordHash = BCrypt.Net.BCrypt.HashPassword(child.PasswordHash);

            _context.Users.Add(child);
            await _context.SaveChangesAsync();
            return child;
        }

        public async Task<User?> GetChildProfileAsync(int childId)
        {
            return await _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == childId && u.Role == "Child");
        }

    }
}
