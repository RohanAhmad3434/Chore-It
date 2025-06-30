using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parent_Child.Services
{
    public interface IFamilyService
    {
        Task<User> AddChildAsync(int parentId, User child);
        Task<List<User>> GetChildrenAsync(int parentId);
        Task<User?> GetChildProfileAsync(int childId); // ✅ Add this line
    }
}
