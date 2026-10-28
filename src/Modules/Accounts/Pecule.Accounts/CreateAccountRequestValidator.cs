using FluentValidation;

using Pecule.Accounts.Contracts;

namespace Pecule.Accounts;

internal sealed class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Account name is required.")
            .MaximumLength(60).WithMessage("Account name must not exceed 60 characters.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid account type.");
    }
}
