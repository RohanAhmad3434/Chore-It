using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;
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

        public async Task<List<ChildDto>> GetChildrenAsync(int parentId)
        {
            if (parentId <= 0)
                throw new ArgumentException("Invalid parentId.");


            // ✅ Validate that the parent exists
            var parentExists = await _context.Users
                .AnyAsync(u => u.Id == parentId && u.Role == "Parent");

            if (!parentExists)
                throw new Exception($"Parent with ID {parentId} not found.");

            var childIds = await _context.ParentChildren
                .Where(pc => pc.ParentId == parentId)
                .Select(pc => pc.ChildId)
                .ToListAsync();

            var children = await _context.Users
                .Where(u => childIds.Contains(u.Id))
                .Include(u => u.Tasks)
                .ToListAsync();

            var tasks = await _context.Tasks
            .Where(t => childIds.Contains(t.AssignedToId ?? 0))
            .ToListAsync();


            return children.Select(c => new ChildDto
            {
                Id = c.Id,
                FullName = c.FullName,
                Email = c.Email,
                Role = c.Role,
                IsGoogleUser = c.IsGoogleUser,
                DateOfBirth = c.DateOfBirth,
                Tasks = tasks
              .Where(t => t.AssignedToId == c.Id)
              .Select(t => new TaskItem
              {
                  Id = t.Id,
                  Title = t.Title,
                  Description = t.Description,
                  Priority = t.Priority,
                  StartTime = t.StartTime,
                  EndTime = t.EndTime,
                  AssignedToId = t.AssignedToId,
                  UserId = t.UserId,
                  RewardId = t.RewardId,
                  IsCompleted = t.IsCompleted,
                  IsApproved = t.IsApproved,
                  CompletionPhotoUrl = t.CompletionPhotoUrl,
                  CompletedOn = t.CompletedOn,
                  IsRedeemed = t.IsRedeemed,
                  RedeemedOn = t.RedeemedOn
              }).ToList()
            }).ToList();

        }

        public async Task<User?> AddChildAsync(int parentId, User child, string relation)
        {
            if (parentId <= 0)
                throw new ArgumentException("Invalid parentId.");

            if (child == null)
                throw new ArgumentException("Child data is required.");

            if (string.IsNullOrWhiteSpace(child.Email))
                throw new ArgumentException("Child email is required.");

            if (string.IsNullOrWhiteSpace(child.PasswordHash))
                throw new ArgumentException("Child password is required.");

            if (string.IsNullOrWhiteSpace(relation))
                throw new ArgumentException("Relation is required.");

            // Check if email already exists
            bool emailExists = await _context.Users.AnyAsync(u => u.Email == child.Email.Trim());
            if (emailExists)
                throw new Exception("A user with this email already exists.");

            // Hash password before saving
            child.PasswordHash = BCrypt.Net.BCrypt.HashPassword(child.PasswordHash);
            child.Role = "Child";

            // Add child user to database
            _context.Users.Add(child);
            await _context.SaveChangesAsync();

            // Create ParentChild mapping
            var parentChild = new ParentChild
            {
                ParentId = parentId,
                ChildId = child.Id,
                Relation = relation
            };

            _context.ParentChildren.Add(parentChild);
            await _context.SaveChangesAsync();

            return child;
        }


        public async Task<User?> GetChildProfileAsync(int childId)
        {
            if (childId <= 0)
                throw new ArgumentException("Invalid childId.");

            var child = await _context.Users
                              .Include(u => u.Tasks)
                              .FirstOrDefaultAsync(u => u.Id == childId && u.Role == "Child");

            if (child == null)
                throw new Exception($"Child with ID {childId} not found.");

            return await _context.Users
                .Include(u => u.Tasks)
                .FirstOrDefaultAsync(u => u.Id == childId && u.Role == "Child");
        }


        public async Task<bool> AssignChildAsync(int parentId, int childId, string relation)
        {
            if (parentId <= 0 || childId <= 0)
                throw new ArgumentException("Invalid parentId or childId.");

            if (string.IsNullOrWhiteSpace(relation))
                throw new ArgumentException("Relation is required.");

            var parent = await _context.Users.FirstOrDefaultAsync(u => u.Id == parentId && u.Role == "Parent");
            if (parent == null)
            {
                 throw new Exception("Parent not found.");
            }

            var child = await _context.Users.FirstOrDefaultAsync(u => u.Id == childId && u.Role == "Child");
            if (child == null)
            {
                 throw new Exception("Child not found.");
            }

            var existing = await _context.ParentChildren
                .AnyAsync(pc => pc.ParentId == parentId && pc.ChildId == childId);

            if (existing)
            {
                 throw new Exception("This child is already assigned to this parent.");
                
            }

            // ✅ Create ParentChild mapping
            var parentChild = new ParentChild
            {
                ParentId = parentId,
                ChildId = childId,
                Relation = relation
            };

            _context.ParentChildren.Add(parentChild);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
