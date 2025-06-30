using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;

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


    //public async Task<List<RewardDto>> GetRewardsByChildIdAsync(int childId)
    //{
    //    return await _context.Tasks
    //        .Where(t => t.AssignedToId == childId && t.RewardId != null)
    //        .Include(t => t.Reward)
    //        .Select(t => new RewardDto
    //        {
    //            Id = t.Reward.Id,
    //            Title = t.Reward.Title,
    //            Description = t.Reward.Description,
    //            AssignedToId = t.AssignedToId
    //        })
    //        .Distinct()
    //        .ToListAsync();
    //}


    public async Task<List<RewardDto>> GetRewardsByChildIdAsync(int childId)
    {
        // ✅ Validate if child exists
        var childExists = await _context.Users.AnyAsync(u => u.Id == childId && u.Role == "Child");
        if (!childExists)
            throw new Exception($"Child with ID {childId} does not exist.");

        // ✅ Fetch rewards linked via tasks
        var rewards = await _context.Tasks
            .Where(t => t.AssignedToId == childId && t.RewardId != null)
            .Include(t => t.Reward)
            .Select(t => new RewardDto
            {
                Id = t.Reward.Id,
                Title = t.Reward.Title,
                Description = t.Reward.Description,
                AssignedToId = t.AssignedToId
            })
            .Distinct()
            .ToListAsync();

        // ✅ Validate if rewards found
        if (rewards.Count == 0)
            throw new Exception($"No rewards found for child ID {childId}.");

        return rewards;
    }

}
