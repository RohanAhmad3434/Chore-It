using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;
using Parent_Child.Services;
using Parent_Child.Models;
public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    //public TaskService(AppDbContext context) => _context = context;

    
    public async Task<List<TaskItem>> GetAllAsync() =>
        await _context.Tasks.Include(t => t.Reward).ToListAsync();

    public TaskService(AppDbContext context)
    {
        _context = context;
        
    }
    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> ApproveTask(int id)
    {
        var task = await _context.Tasks
            .Include(t => t.Reward)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return false;

        task.IsApproved = true;
        task.IsCompleted = true;

        // ✅ If the task has a reward, mark it as redeemed at approval time
        if (task.RewardId != null)
        {
            task.IsRedeemed = true;
            task.RedeemedOn = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return true;
    }



    public async Task<bool> RejectTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;
        task.IsApproved = false;
        task.IsCompleted = false; // redo
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskItemDto>> GetTasksByChildIdAsync(int childId)
    {
        return await _context.Tasks
            .Where(t => t.AssignedToId == childId)
            .Include(t => t.AssignedTo)
            .Include(t => t.User)
            .Include(t => t.Reward)
            .Select(t => new TaskItemDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Priority = t.Priority,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                AssignedToId = t.AssignedToId,
                AssignedToName = t.AssignedTo != null ? t.AssignedTo.FullName : null,
                UserId = t.UserId,
                UserName = t.User != null ? t.User.FullName : null,
                RewardId = t.RewardId,
                RewardTitle = t.Reward != null ? t.Reward.Title : null,
                IsCompleted = t.IsCompleted,
                IsApproved = t.IsApproved
            })
            .ToListAsync();
    }


    public async Task<bool> MarkTaskCompleted(int childId, int taskId, IFormFile photoFile)
    {
        // ✅ Validate input
        if (photoFile == null || photoFile.Length == 0)
        {
            // ❌ No photo uploaded
            return false;
        }

        // ✅ Fetch task assigned to this child
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedToId == childId);

        if (task == null)
        {
            // ❌ Task not found or not assigned to this child
            return false;
        }

        // ✅ If task already completed, skip update
        if (task.IsCompleted)
        {
            return true; // already marked as complete
        }

        // ✅ Prepare uploads directory path
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // ✅ Generate unique filename and save the photo
        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await photoFile.CopyToAsync(fileStream);
        }

        // ✅ Mark task as completed with photo URL (relative path for your app)
        task.IsCompleted = true;
        task.CompletedOn = DateTime.UtcNow;
        task.CompletionPhotoUrl = "/uploads/" + uniqueFileName;
      

        await _context.SaveChangesAsync();

        //// ✅ Check and assign achievements for this child
        //await _achievementService.CheckAndAssignAchievements(childId);

        return true;
    }


    // ✅ 1. Get active tasks for a child
    public async Task<List<TaskItem>> GetActiveTasksForChildAsync(int childId)
    {
        return await _context.Tasks
            .Where(t => t.AssignedToId == childId && !t.IsCompleted)
            .Include(t => t.Reward)
            .ToListAsync();
    }
    public async Task<bool> CheckChildExistsAsync(int childId)
    {
        return await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
    }


}
