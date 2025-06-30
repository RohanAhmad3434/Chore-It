using System.Collections.Generic;
using System.Threading.Tasks;
using Parent_Child.DTOs;

public interface ITaskService
{
    Task<List<TaskItem>> GetAllAsync();
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<bool> ApproveTask(int id);
    Task<bool> RejectTask(int id);

    // ✅ Updated to match DTO
    Task<List<TaskItemDto>> GetTasksByChildIdAsync(int childId);
    Task<bool> MarkTaskCompleted(int childId, int taskId);
    Task<bool> CheckChildExistsAsync(int childId);


}
