using FluentValidation;

namespace CareerOS.Application.Features.Wiki.Commands.CreateWikiArticle;

public class CreateWikiArticleCommandValidator : AbstractValidator<CreateWikiArticleCommand>
{
    public CreateWikiArticleCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content cannot be null.");
    }
}

