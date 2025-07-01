using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;
using Parent_Child.Services; // Ensure namespace is correct

public class StatisticsService : IStatsService
{
    private readonly AppDbContext _context;

    public StatisticsService(AppDbContext context)
    {
        _context = context;
    }

    // ✅ 5. Get child task and reward statistics
    public async Task<ChildStatisticsDto> GetChildStatsAsync(int childId)
    {
        var totalTasks = await _context.Tasks.CountAsync(t => t.AssignedToId == childId);
        var completedTasks = await _context.Tasks.CountAsync(t => t.AssignedToId == childId && t.IsCompleted);

        var totalRewards = await _context.Rewards.CountAsync(r => r.AssignedToId == childId);
        var redeemedRewards = await _context.Rewards.CountAsync(r => r.AssignedToId == childId /* && r.IsRedeemed */); // Adjust if you implement IsRedeemed

        return new ChildStatisticsDto
        {
            TaskCompletionRate = totalTasks == 0 ? 0 : (int)((double)completedTasks / totalTasks * 100),
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            RewardsReceived = totalRewards,
            RewardsRedeemed = redeemedRewards
        };
    }
}
