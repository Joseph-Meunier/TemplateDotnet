using FluentValidation;

namespace Template.Modules.Blog.Features.CreatePost;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.AuthorUserId)
            .NotEmpty();

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

        RuleFor(x => x.HeroImage)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.HeroImage));
    }
}