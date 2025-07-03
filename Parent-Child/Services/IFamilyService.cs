using System.Collections.Generic;
using System.Threading.Tasks;
using Parent_Child.DTOs;
using Parent_Child.Models;
namespace Parent_Child.Services
{
    public interface IFamilyService
    {
        Task<User?> AddChildAsync(int parentId, User child, string relation);
        Task<List<ChildDto>> GetChildrenAsync(int parentId);
        Task<User?> GetChildProfileAsync(int childId);
        Task<bool> AssignChildAsync(int parentId, int childId, string relation);

    }
}
