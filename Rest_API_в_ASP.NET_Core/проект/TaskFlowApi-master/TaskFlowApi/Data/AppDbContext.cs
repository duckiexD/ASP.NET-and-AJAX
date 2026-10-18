using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using TaskFlowApi.Entities;

namespace TaskFlowApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Comment> Comments => Set<Comment>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        // Project -> TaskItem (1:M) каскад
        mb.Entity<TaskItem>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // TaskItem -> Comment (1:M) каскад
        mb.Entity<Comment>()
            .HasOne(c => c.TaskItem)
            .WithMany(t => t.Comments)
            .HasForeignKey(c => c.TaskItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> TaskItem (AssignedTo)
        mb.Entity<TaskItem>()
            .HasOne(t => t.AssignedTo)
            .WithMany(u => u.AssignedTasks)
            .HasForeignKey(t => t.AssignedToId)
            .OnDelete(DeleteBehavior.SetNull);

        // User -> Comment (Author)
        mb.Entity<Comment>()
            .HasOne(c => c.Author)
            .WithMany(u => u.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Индекс по статусу
        mb.Entity<TaskItem>().HasIndex(t => t.Status);

        // Seed data
        mb.Entity<AppUser>().HasData(new AppUser
        {
            Id = 1,
            Username = "alice",
            Email = "alice@example.com",
            PasswordHash = "HASH"
        });

        mb.Entity<Project>().HasData(new Project
        {
            Id = 1,
            Name = "Demo Project",
            Description = "Seed project",
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        mb.Entity<TaskItem>().HasData(new TaskItem
        {
            Id = 1,
            ProjectId = 1,
            Title = "Seed task example",
            Status =  TaskItemStatus.ToDo,
            Priority = TaskPriority.Medium,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}