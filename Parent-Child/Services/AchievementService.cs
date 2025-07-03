using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;
using Parent_Child.Models;

namespace Parent_Child.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AchievementService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<AchievementDto> CreateAchievementAsync(AchievementCreateDto dto, HttpRequest request)
        {
            string? iconUrl = null;

            // ✅ Handle file upload
            if (dto.IconFile != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.IconFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.IconFile.CopyToAsync(stream);
                }

                // Generate URL
                iconUrl = $"{request.Scheme}://{request.Host}/uploads/{fileName}";
            }

            var achievement = new Achievement
            {
                Title = dto.Title,
                Description = dto.Description,
                RewardId = dto.RewardId,
                CompletionThreshold = dto.CompletionThreshold,
                IconUrl = iconUrl
            };

            _context.Achievements.Add(achievement);
            await _context.SaveChangesAsync();

            var reward = await _context.Rewards.FindAsync(dto.RewardId);

            return new AchievementDto
            {
                Id = achievement.Id,
                Title = achievement.Title,
                Description = achievement.Description,
                RewardId = achievement.RewardId,
                RewardTitle = reward?.Title ?? "",
                CompletionThreshold = achievement.CompletionThreshold,
                IconUrl = achievement.IconUrl
            };
        }

        public async Task<List<Achievement>> GetAllAchievementsAsync()
        {
            return await _context.Achievements
                .Include(a => a.Reward)
                .ToListAsync();
        }


        public async Task<List<UserAchievementDto>> GetChildAchievementsAsync(int childId)
        {
            // ✅ Check if user exists and is a Child
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == childId && u.Role == "Child");

            if (user == null)
                throw new Exception($"Child with ID {childId} does not exist or is not a child.");

            var achievements = await _context.UserAchievements
                .Where(ua => ua.UserId == childId)
                .Include(ua => ua.Achievement)
                .ToListAsync();

            return achievements.Select(a => new UserAchievementDto
            {
                Id = a.Id,
                AchievementId = a.AchievementId,
                AchievementTitle = a.Achievement.Title,
                IconUrl = a.Achievement.IconUrl,
                DateAchieved = a.DateAchieved
            }).ToList();
        }



        public async Task<AchievementDto> UpdateAchievementAsync(int id, AchievementUpdateDto dto, HttpRequest request)
        {
            var achievement = await _context.Achievements.FindAsync(id);

            if (achievement == null)
                throw new Exception($"Achievement with ID {id} not found.");

            // ✅ Handle updated icon upload if provided
            if (dto.IconFile != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.IconFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.IconFile.CopyToAsync(stream);
                }

                achievement.IconUrl = $"{request.Scheme}://{request.Host}/uploads/{fileName}";
            }

            // ✅ Update other fields
            achievement.Title = dto.Title;
            achievement.Description = dto.Description;
            achievement.RewardId = dto.RewardId;
            achievement.CompletionThreshold = dto.CompletionThreshold;

            await _context.SaveChangesAsync();

            var reward = await _context.Rewards.FindAsync(dto.RewardId);

            return new AchievementDto
            {
                Id = achievement.Id,
                Title = achievement.Title,
                Description = achievement.Description,
                RewardId = achievement.RewardId,
                RewardTitle = reward?.Title ?? "",
                CompletionThreshold = achievement.CompletionThreshold,
                IconUrl = achievement.IconUrl
            };
        }

        public async Task<bool> DeleteAchievementAsync(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);

            if (achievement == null)
                throw new Exception($"Achievement with ID {id} not found.");

            _context.Achievements.Remove(achievement);
            await _context.SaveChangesAsync();

            return true;
        }



    }
}
