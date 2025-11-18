namespace Project.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Navigation Properties
    public virtual ICollection<TaskItem>? Tasks { get; set; }

    // Computed property (not mapped to DB)
    public string FullName => $"{FirstName} {LastName}";
}
