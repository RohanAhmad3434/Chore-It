using Parent_Child.DTOs;
using Parent_Child.Models;

namespace Parent_Child.Services
{
    public interface IAchievementService
    {
        Task<List<Achievement>> GetAllAchievementsAsync();
        Task<AchievementDto> CreateAchievementAsync(AchievementCreateDto dto, HttpRequest request);
        Task<List<UserAchievementDto>> GetChildAchievementsAsync(int childId);

        Task<AchievementDto> UpdateAchievementAsync(int id, AchievementUpdateDto dto, HttpRequest request);

        Task<bool> DeleteAchievementAsync(int id);
    }
}
