using FluentValidation;

namespace CareerOS.Application.Features.Interviews.Commands.CreateInterview;

public class CreateInterviewCommandValidator : AbstractValidator<CreateInterviewCommand>
{
    public CreateInterviewCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0).WithMessage("Duration must be greater than 0 minutes.");
    }
}
