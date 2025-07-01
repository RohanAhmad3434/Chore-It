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

        // ✅ Fetch rewards linked via tasks
        var rewardsFromTasks = await _context.Tasks
            .Where(t => t.AssignedToId == childId && t.RewardId != null)
            .Include(t => t.Reward)
            .Select(t => new RewardDto
            {
                Id = t.Reward.Id,
                Title = t.Reward.Title,
                Description = t.Reward.Description,
                AssignedToId = t.AssignedToId,
                IsRedeemed = t.Reward.IsRedeemed,
                RedeemedOn = t.Reward.RedeemedOn
            })
            .ToListAsync();

        // ✅ Fetch rewards assigned directly to child
        var directlyAssignedRewards = await _context.Rewards
            .Where(r => r.AssignedToId == childId)
            .Select(r => new RewardDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                AssignedToId = r.AssignedToId,
                IsRedeemed = r.IsRedeemed,
                RedeemedOn = r.RedeemedOn
            })
            .ToListAsync();

        // ✅ Combine both sources without duplicates
        var allRewards = rewardsFromTasks.Concat(directlyAssignedRewards)
            .GroupBy(r => r.Id)
            .Select(g => g.First())
            .ToList();

        // ✅ Validate if rewards found
        if (allRewards.Count == 0)
            throw new Exception($"No rewards found for child ID {childId}.");

        return allRewards;
    }


    // ✅ 4. Redeem reward
    public async Task<Reward?> RedeemRewardAsync(int rewardId)
    {
        var reward = await _context.Rewards.FindAsync(rewardId);
        if (reward == null || reward.IsRedeemed)
            return null;

        reward.IsRedeemed = true;
        reward.RedeemedOn = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return reward;
    }

}
