using Microsoft.EntityFrameworkCore;
using Parent_Child.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaskItem>()
           .HasOne(t => t.Reward)
           .WithMany()
           .HasForeignKey(t => t.RewardId)
           .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<TaskItem>()
    .HasOne(t => t.User)
    .WithMany(u => u.Tasks)
    .HasForeignKey(t => t.UserId)
    .OnDelete(DeleteBehavior.Cascade);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Reward> Rewards { get; set; }
    public DbSet<TaskItem> Tasks { get; set; }
 
}