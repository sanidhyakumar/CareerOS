using FluentValidation;

namespace CareerOS.Application.Features.Wiki.Commands.UpdateWikiArticle;

public class UpdateWikiArticleCommandValidator : AbstractValidator<UpdateWikiArticleCommand>
{
    public UpdateWikiArticleCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Article ID is required.");

        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Content)
            .NotNull().WithMessage("Content cannot be null.");
    }
}
