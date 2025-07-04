using Microsoft.EntityFrameworkCore;
using Parent_Child.DTOs;
using Parent_Child.Models;

namespace Parent_Child.Services
{
    public class AchievementService : IAchievementService
    {
        private readonly AppDbContext _context;
        public AchievementService(AppDbContext context)
        {
            _context = context;
        }


        public async Task<AchievementDto> CreateAchievementAsync(AchievementCreateDto dto, HttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required.");

            if (dto.RewardId <= 0)
                throw new ArgumentException("RewardId must be valid.");

            if (dto.CompletionThreshold <= 0)
                throw new ArgumentException("CompletionThreshold must be greater than zero.");

            var rewardExists = await _context.Rewards.AnyAsync(r => r.Id == dto.RewardId);
            if (!rewardExists)
                throw new Exception($"Reward with ID {dto.RewardId} does not exist.");



            string? iconPath = null;

            if (dto.IconFile != null)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.IconFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.IconFile.CopyToAsync(stream);
                }

                iconPath = "/uploads/" + fileName;
            }

            var achievement = new Achievement
            {
                Title = dto.Title,
                Description = dto.Description,
                RewardId = dto.RewardId,
                CompletionThreshold = dto.CompletionThreshold,
                IconUrl = iconPath
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

            if (string.IsNullOrWhiteSpace(dto.Title))
                throw new ArgumentException("Title is required.");

            if (dto.RewardId <= 0)
                throw new ArgumentException("RewardId must be valid.");

            if (dto.CompletionThreshold <= 0)
                throw new ArgumentException("CompletionThreshold must be greater than zero.");

            var rewardExists = await _context.Rewards.AnyAsync(r => r.Id == dto.RewardId);
            if (!rewardExists)
                throw new Exception($"Reward with ID {dto.RewardId} does not exist.");


            // ✅ If a new icon file is uploaded
            if (dto.IconFile != null)
            {
                //// ✅ Delete previous icon file if exists and is stored as relative path
                //if (!string.IsNullOrEmpty(achievement.IconUrl))
                //{
                //    var existingFilePath = Path.Combine(Directory.GetCurrentDirectory(), achievement.IconUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                //    if (System.IO.File.Exists(existingFilePath))
                //    {
                //        System.IO.File.Delete(existingFilePath);
                //    }
                //}

                // ✅ Save new uploaded file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.IconFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.IconFile.CopyToAsync(stream);
                }

                achievement.IconUrl = "/uploads/" + fileName;
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
