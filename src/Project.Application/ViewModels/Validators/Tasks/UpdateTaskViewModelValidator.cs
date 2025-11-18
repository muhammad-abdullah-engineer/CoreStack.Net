using FluentValidation;
using Project.Application.ViewModels.Tasks;

namespace Project.Application.ViewModels.Validators.Tasks;

public class UpdateTaskViewModelValidator : AbstractValidator<UpdateTaskViewModel>
{
    public UpdateTaskViewModelValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Status)
            .InclusiveBetween(0, 3)
            .WithMessage("Status must be between 0 (Todo) and 3 (Cancelled)");

        RuleFor(x => x.Priority)
            .InclusiveBetween(0, 3)
            .WithMessage("Priority must be between 0 (Low) and 3 (Urgent)");
    }
}
