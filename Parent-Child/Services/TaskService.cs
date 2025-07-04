using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;
using Parent_Child.Services;
using Parent_Child.Models;
public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    public TaskService(AppDbContext context)
    {
        _context = context;
        
    }

    public async Task<List<TaskItem>> GetAllAsync() =>
        await _context.Tasks.Include(t => t.Reward).ToListAsync();

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        // ✅ Validate title
        if (string.IsNullOrWhiteSpace(task.Title))
            throw new ArgumentException("Task title is required.");

        // ✅ Validate start and end time consistency
        if (task.StartTime >= task.EndTime)
            throw new ArgumentException("Start time must be before end time.");

        // ✅ Validate assigned child existence (if provided)
        if (task.AssignedToId != null)
        {
            var childExists = await _context.Users.AnyAsync(u => u.Id == task.AssignedToId && u.Role == "Child");
            if (!childExists)
                throw new Exception($"Assigned child with ID {task.AssignedToId} does not exist.");
        }

        // ✅ Validate parent existence
        var parentExists = await _context.Users.AnyAsync(u => u.Id == task.UserId && u.Role == "Parent");
        if (!parentExists)
            throw new Exception($"Parent with ID {task.UserId} does not exist.");

        // ✅ Validate reward existence if provided
        if (task.RewardId != null)
        {
            var rewardExists = await _context.Rewards.AnyAsync(r => r.Id == task.RewardId);
            if (!rewardExists)
                throw new Exception($"Reward with ID {task.RewardId} does not exist.");
        }


        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> ApproveTask(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid task ID.");

        var task = await _context.Tasks
            .Include(t => t.Reward)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null)
            throw new Exception($"Task with ID {id} not found.");

        task.IsApproved = true;
        task.IsCompleted = true;

        // ✅ If the task has a reward, mark it as redeemed at approval time
        if (task.RewardId != null)
        {
            task.IsRedeemed = true;
            task.RedeemedOn = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        // ✅ Call achievement check logic
        await CheckAndAssignAchievementsAsync(task.AssignedToId ?? 0);

        return true;
    }



    private async Task CheckAndAssignAchievementsAsync(int childId)
    {
        if (childId <= 0)
            return;

        var childExists = await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
        if (!childExists)
            throw new Exception($"Child with ID {childId} does not exist.");

        // Get all rewards the child has completed
        var completedRewards = await _context.Tasks
            .Where(t => t.AssignedToId == childId && t.IsCompleted && t.IsApproved && t.RewardId != null)
            .GroupBy(t => t.RewardId)
            .Select(g => new
            {
                RewardId = g.Key.Value,
                Count = g.Count()
            })
            .ToListAsync();

        // Get all achievements definitions
        var achievements = await _context.Achievements.ToListAsync();

        foreach (var rewardCompletion in completedRewards)
        {
            var matchingAchievements = achievements
                .Where(a => a.RewardId == rewardCompletion.RewardId &&
                            rewardCompletion.Count >= a.CompletionThreshold)
                .ToList();

            foreach (var achievement in matchingAchievements)
            {
                // Check if child already has this achievement
                bool alreadyHas = await _context.UserAchievements
                    .AnyAsync(ua => ua.UserId == childId && ua.AchievementId == achievement.Id);

                if (!alreadyHas)
                {
                    // Assign achievement
                    var userAchievement = new UserAchievement
                    {
                        UserId = childId,
                        AchievementId = achievement.Id,
                        DateAchieved = DateTime.UtcNow
                    };

                    _context.UserAchievements.Add(userAchievement);
                }
            }
        }

        await _context.SaveChangesAsync();
    }



    public async Task<bool> RejectTask(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid task ID.");

        var task = await _context.Tasks.FindAsync(id);
        if (task == null)
            throw new Exception($"Task with ID {id} not found.");

        task.IsApproved = false;
        task.IsCompleted = false; // redo
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<TaskItemDto>> GetTasksByChildIdAsync(int childId)
    {
        if (childId <= 0)
            throw new ArgumentException("Invalid child ID.");

        var childExists = await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
        if (!childExists)
            throw new Exception($"Child with ID {childId} does not exist.");

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
        // ✅ Validate child existence
        var childExists = await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
        if (!childExists)
            throw new Exception($"Child with ID {childId} does not exist.");


        // ✅ Validate input photo
        if (photoFile == null || photoFile.Length == 0)
            throw new ArgumentException("No photo uploaded.");


        // ✅ Fetch task assigned to this child
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedToId == childId);

        if (task == null)
            throw new Exception($"Task with ID {taskId} not found for child ID {childId}.");


        // ✅ Prepare uploads directory path
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        // ✅ Delete existing photo file if it exists
        //if (!string.IsNullOrEmpty(task.CompletionPhotoUrl))
        //{
        //    var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), task.CompletionPhotoUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
        //    if (System.IO.File.Exists(existingFilePath))
        //    {
        //        System.IO.File.Delete(existingFilePath);
        //    }
        //}

        // ✅ Generate unique filename and save the new photo
        var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await photoFile.CopyToAsync(fileStream);
        }

        // ✅ Mark task as completed with new photo URL
        task.IsCompleted = true;
        task.CompletedOn = DateTime.UtcNow;
        task.CompletionPhotoUrl = "/uploads/" + uniqueFileName;

        await _context.SaveChangesAsync();
        return true;
    }



    // ✅ 1. Get active tasks for a child
    public async Task<List<TaskItem>> GetActiveTasksForChildAsync(int childId)
    {
        if (childId <= 0)
            throw new ArgumentException("Invalid child ID.");

        var childExists = await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
        if (!childExists)
            throw new Exception($"Child with ID {childId} does not exist.");

        return await _context.Tasks
            .Where(t => t.AssignedToId == childId && !t.IsCompleted)
            .Include(t => t.Reward)
            .ToListAsync();
    }
    public async Task<bool> CheckChildExistsAsync(int childId)
    {
        if (childId <= 0)
            throw new ArgumentException("Invalid child ID.");

        var childExists = await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
        if (!childExists)
            throw new Exception($"Child with ID {childId} does not exist.");
        return await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
    }


}
