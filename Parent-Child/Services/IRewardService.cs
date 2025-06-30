using System.Collections.Generic;
using System.Threading.Tasks;
using Parent_Child.DTOs;

public interface IRewardService
{
    Task<List<Reward>> GetAllAsync();
    Task<Reward> CreateAsync(Reward reward);

    // ✅ NEW
    //Task<List<Reward>> GetRewardsByChildIdAsync(int childId);

    //Task<List<Reward>> GetRewardsByChildIdAsync(int childId);
    Task<List<RewardDto>> GetRewardsByChildIdAsync(int childId);


}
