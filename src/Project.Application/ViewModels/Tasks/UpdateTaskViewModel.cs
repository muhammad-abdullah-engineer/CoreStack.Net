namespace Project.Application.ViewModels.Tasks;

public class UpdateTaskViewModel
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Status { get; set; }
    public int Priority { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Tags { get; set; }
}
