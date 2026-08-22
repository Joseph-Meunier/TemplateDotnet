using FluentValidation;

namespace Template.Modules.Sample.Features.Echo;

public sealed class Validator : AbstractValidator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);
    }
}