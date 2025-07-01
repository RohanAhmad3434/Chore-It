using System.Collections.Generic;
using System.Threading.Tasks;
using Parent_Child.DTOs;
using Parent_Child.Models;

public interface IRewardService
{
    Task<List<Reward>> GetAllAsync();
    Task<Reward> CreateAsync(Reward reward);

    Task<List<RewardDto>> GetRewardsByChildIdAsync(int childId);
    Task<Reward?> RedeemRewardAsync(int rewardId);

}
