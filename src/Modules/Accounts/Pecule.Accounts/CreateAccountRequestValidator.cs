using FluentValidation;

using Pecule.Accounts.Contracts;

namespace Pecule.Accounts;

internal sealed class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Le nom du compte est obligatoire.")
            .MaximumLength(60).WithMessage("Le nom du compte ne peut excéder 60 caractères.");
        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Type de compte invalide.");
    }
}
