using FluentValidation;
using Project.Application.ViewModels.Tasks;

namespace Project.Application.ViewModels.Validators.Tasks;

public class CreateTaskViewModelValidator : AbstractValidator<CreateTaskViewModel>
{
    public CreateTaskViewModelValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 3)
            .WithMessage("Priority must be between 0 (Low) and 3 (Urgent)");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
            .When(x => x.DueDate.HasValue)
            .WithMessage("Due date cannot be in the past");
    }
}
