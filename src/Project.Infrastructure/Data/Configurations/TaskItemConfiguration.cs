using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Project.Domain.Entities;

namespace Project.Infrastructure.Data.Configurations;

public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Tasks");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(2000);
        builder.Property(t => t.Status).IsRequired().HasConversion<int>();
        builder.Property(t => t.Priority).IsRequired().HasConversion<int>();
        builder.Property(t => t.Tags).HasMaxLength(500);
        builder.Property(t => t.CreatedAt).IsRequired().HasDefaultValueSql("CURRENT_TIMESTAMP");
        builder.Property(t => t.IsDeleted).IsRequired().HasDefaultValue(false);

        builder.HasIndex(t => t.UserId).HasDatabaseName("IX_Tasks_UserId");
        builder.HasIndex(t => t.Status).HasDatabaseName("IX_Tasks_Status");
        builder.HasIndex(t => t.DueDate).HasDatabaseName("IX_Tasks_DueDate");
        builder.HasIndex(t => new { t.UserId, t.Status }).HasDatabaseName("IX_Tasks_UserId_Status");

        builder.HasQueryFilter(t => !t.IsDeleted);

        // Configure relationships
        builder.HasOne(t => t.User)
               .WithMany(u => u.Tasks)
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
