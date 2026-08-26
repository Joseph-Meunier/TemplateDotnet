using FluentValidation;

namespace Template.Modules.Blog.Features.CreatePost;

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

        RuleFor(x => x.HeroImage)
            .MaximumLength(500)
            .When(x => !string.IsNullOrWhiteSpace(x.HeroImage));
        
        RuleFor(x => x.Tags)
            .NotNull();

        RuleForEach(x => x.Tags)
            .NotEmpty()
            .MaximumLength(100);
        
        RuleFor(x => x.Tags)
            .Must(tags => tags.Count <= 20)
            .WithMessage("A post cannot have more than 20 tags.");
    }
}