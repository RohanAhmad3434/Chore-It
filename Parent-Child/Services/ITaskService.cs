using System.Collections.Generic;
using System.Threading.Tasks;
using Parent_Child.DTOs;
using Parent_Child.Models;

public interface ITaskService
{
    Task<List<TaskItem>> GetAllAsync();
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<bool> ApproveTask(int id);
    Task<bool> RejectTask(int id);

    Task<List<TaskItemDto>> GetTasksByChildIdAsync(int childId);

    Task<bool> MarkTaskCompleted(int childId, int taskId, IFormFile photoFile);
    Task<bool> CheckChildExistsAsync(int childId);
    Task<List<TaskItem>> GetActiveTasksForChildAsync(int childId);

}
