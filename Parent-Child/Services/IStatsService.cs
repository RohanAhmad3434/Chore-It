using Parent_Child.DTOs;

namespace Parent_Child.Services
{
    public interface IStatsService
    {
        Task<ChildStatisticsDto> GetChildStatsAsync(int childId);
    }
}
