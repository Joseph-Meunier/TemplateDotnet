using FluentValidation;

namespace Template.Modules.Blog.Features.UpdatePost;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Content)
            .NotEmpty();

        RuleFor(x => x.ReadingTimeMinutes)
            .GreaterThan(0);

        RuleFor(x => x.Tags)
            .NotNull()
            .Must(tags => tags.Count <= 20)
            .WithMessage("A post cannot have more than 20 tags.");

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(100);
    }
}