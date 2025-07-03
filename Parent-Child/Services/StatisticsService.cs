using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;
using Parent_Child.Models;
using Parent_Child.Services;

public class StatisticsService : IStatsService
{
    private readonly AppDbContext _context;

    public StatisticsService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ChildStatisticsDto> GetChildStatsAsync(int childId)
    {
        var totalTasks = await _context.Tasks.CountAsync(t => t.AssignedToId == childId);
        var completedTasks = await _context.Tasks.CountAsync(t => t.AssignedToId == childId && t.IsApproved);

        var totalRewards = await _context.Tasks.CountAsync(t => t.AssignedToId == childId && t.RewardId != null);
        var redeemedRewards = await _context.Tasks.CountAsync(t => t.AssignedToId == childId && t.RewardId != null && t.IsRedeemed);

        var rewardsCompletionRate = totalRewards == 0 ? 0 : (int)((double)redeemedRewards / totalRewards * 100);

        // ✅ Fetch parent-child relations from ParentChild mapping table
        var parentRelations = await _context.ParentChildren
            .Where(pc => pc.ChildId == childId)
            .Include(pc => pc.Parent)
            .Select(pc => new
            {
                ParentId = pc.ParentId,
                Relation = pc.Relation ?? "Unknown"
            })
            .ToListAsync();

        var relationStats = new List<RelationTaskStatsDto>();

        foreach (var parent in parentRelations)
        {
            var tasksCount = await _context.Tasks
                .CountAsync(t => t.AssignedToId == childId && t.UserId == parent.ParentId);

            var tasksCompleted = await _context.Tasks
                .CountAsync(t => t.AssignedToId == childId && t.UserId == parent.ParentId && t.IsApproved);

            var completionRate = tasksCount == 0 ? 0 : (int)((double)tasksCompleted / tasksCount * 100);

            relationStats.Add(new RelationTaskStatsDto
            {
                Relation = parent.Relation,
                TasksCount = tasksCount,
                TasksCompleted = tasksCompleted,
                CompletionRate = completionRate
            });
        }

        return new ChildStatisticsDto
        {
            TaskCompletionRate = totalTasks == 0 ? 0 : (int)((double)completedTasks / totalTasks * 100),
            RewardsCompletionRate = rewardsCompletionRate,
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            RewardsReceived = totalRewards,
            RewardsRedeemed = redeemedRewards,
            TasksByRelations = relationStats
        };
    }
}
