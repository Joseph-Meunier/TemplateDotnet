using FluentValidation;

namespace Template.Modules.Sample.Features.CreateSampleItem;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);
    }
}