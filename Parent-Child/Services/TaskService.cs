using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;

public class TaskService : ITaskService
{
    private readonly AppDbContext _context;
    public TaskService(AppDbContext context) => _context = context;

    public async Task<List<TaskItem>> GetAllAsync() =>
        await _context.Tasks.Include(t => t.Reward).ToListAsync();

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<bool> ApproveTask(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;
        task.IsApproved = true;
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



    // ✅ NEW: Mark task as completed
    public async Task<bool> MarkTaskCompleted(int childId, int taskId)
    {
        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == taskId && t.AssignedToId == childId);

        if (task == null) return false;

        task.IsCompleted = true;
        await _context.SaveChangesAsync();
        return true;
    }



    public async Task<bool> CheckChildExistsAsync(int childId)
    {
        return await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
    }


}
