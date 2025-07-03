using Microsoft.EntityFrameworkCore;
using Parent_Child.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
    public DbSet<ParentChild> ParentChildren { get; set; } // ✅ Add ParentChild DbSet

    public DbSet<Achievement> Achievements { get; set; }
public DbSet<UserAchievement> UserAchievements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ✅ Unique Email for Users
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // ✅ TaskItem -> Reward (nullable)
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.Reward)
            .WithMany()
            .HasForeignKey(t => t.RewardId)
            .OnDelete(DeleteBehavior.SetNull);

        // ✅ TaskItem -> User (creator/assigner)
        modelBuilder.Entity<TaskItem>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tasks)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ✅ Many-to-Many ParentChild configuration
        modelBuilder.Entity<ParentChild>()
            .HasKey(pc => new { pc.ParentId, pc.ChildId });

        modelBuilder.Entity<ParentChild>()
            .HasOne(pc => pc.Parent)
            .WithMany(u => u.Children)
            .HasForeignKey(pc => pc.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ParentChild>()
            .HasOne(pc => pc.Child)
            .WithMany(u => u.Parents)
            .HasForeignKey(pc => pc.ChildId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

