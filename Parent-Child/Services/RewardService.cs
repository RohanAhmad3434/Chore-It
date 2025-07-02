using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;
using Parent_Child.Models;
public class RewardService : IRewardService
{
    private readonly AppDbContext _context;
    public RewardService(AppDbContext context) => _context = context;

    public async Task<List<Reward>> GetAllAsync() =>
        await _context.Rewards.ToListAsync();

    public async Task<Reward> CreateAsync(Reward reward)
    {
        _context.Rewards.Add(reward);
        await _context.SaveChangesAsync();
        return reward;
    }

    public async Task<List<RewardDto>> GetRewardsByChildIdAsync(int childId)
    {
        // ✅ Validate if child exists
        var childExists = await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
        if (!childExists)
            throw new Exception($"Child with ID {childId} does not exist.");

        // ✅ Fetch rewards linked via tasks for this child
        var rewardsFromTasks = await _context.Tasks
            .Where(t => t.AssignedToId == childId && t.RewardId != null && t.IsRedeemed)
            .Include(t => t.Reward)
            .Select(t => new RewardDto
            {
                Id = t.Reward.Id,
                Title = t.Reward.Title,
                Description = t.Reward.Description,
                AssignedToId = t.AssignedToId,
                IsRedeemed = t.IsRedeemed,
                RedeemedOn = t.RedeemedOn
            })
            .ToListAsync();

        // ✅ Remove direct assignment rewards logic since Reward no longer stores AssignedToId

        if (rewardsFromTasks.Count == 0)
            throw new Exception($"No rewards found for child ID {childId}.");

        return rewardsFromTasks;
    }




    //✅ 4. Redeem reward
    public async Task<TaskItem?> RedeemRewardAsync(int taskId)
    {
        var task = await _context.Tasks
            .Include(t => t.Reward)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null || task.IsRedeemed)
            return null;

        task.IsRedeemed = true;
        task.RedeemedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return task;
    }


}
