using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Data;

public class TaskFlowDbContext : DbContext
{
    public TaskFlowDbContext(DbContextOptions<TaskFlowDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.Property(u => u.FullName).IsRequired().HasMaxLength(100);
            e.Property(u => u.Email).IsRequired().HasMaxLength(200);
            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.Property(p => p.Name).IsRequired().HasMaxLength(100);
            e.Property(p => p.Description).HasMaxLength(1000);

            // User 1 -> N Project; nie usuwamy u¿ytkownika, który ma projekty
            e.HasOne(p => p.Owner)
             .WithMany(u => u.OwnedProjects)
             .HasForeignKey(p => p.OwnerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<TaskItem>(e =>
        {
            e.Property(t => t.Title).IsRequired().HasMaxLength(200);
            e.Property(t => t.Description).HasMaxLength(2000);

            // Project 1 -> N TaskItem; usuniêcie projektu usuwa jego zadania
            e.HasOne(t => t.Project)
             .WithMany(p => p.Tasks)
             .HasForeignKey(t => t.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);

            // User 0..1 -> N TaskItem; usuniêcie u¿ytkownika odpina zadania
            e.HasOne(t => t.Assignee)
             .WithMany(u => u.AssignedTasks)
             .HasForeignKey(t => t.AssigneeId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // dane startowe
        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, FullName = "Jan Kowalski", Email = "jan@example.com" });

        modelBuilder.Entity<Project>().HasData(
            new Project
            {
                Id = 1,
                Name = "Pierwszy projekt",
                OwnerId = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            });
    }
}