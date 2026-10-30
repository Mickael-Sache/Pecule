import { Component, inject, signal } from '@angular/core';
import { FormField, FormRoot, required, email, form, minLength, pattern, validate, FieldTree } from '@angular/forms/signals';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Auth } from '../../core/auth/auth';
import { HttpErrorResponse } from '@angular/common/http';

interface RegisterForm {
  email: string;
  password: string;
  confirmPassword: string;
}

interface ValidationProblemBody {
  errors?: Record<string, string[]>;
}

type RegisterField = 'email' | 'password';

const IDENTITY_ERRORS: Record<string, { field: RegisterField; message: string }> = {
  DuplicateUserName: { field: 'email', message: 'Un compte existe déjà avec cette adresse e-mail.' },
  DuplicateEmail: { field: 'email', message: 'Un compte existe déjà avec cette adresse e-mail.' },
  InvalidEmail: { field: 'email', message: 'Saisissez une adresse e-mail valide, par exemple nom@exemple.fr.' },
  PasswordTooShort: { field: 'password', message: 'Le mot de passe doit contenir au moins 12 caractères.' },
  PasswordRequiresUpper: { field: 'password', message: 'Le mot de passe doit contenir au moins une lettre majuscule.' },
  PasswordRequiresLower: { field: 'password', message: 'Le mot de passe doit contenir au moins une lettre minuscule.' },
  PasswordRequiresDigit: { field: 'password', message: 'Le mot de passe doit contenir au moins un chiffre.' },
  PasswordRequiresNonAlphanumeric: { field: 'password', message: 'Le mot de passe doit contenir au moins un caractère spécial, par exemple ! ou ?.' },
};

@Component({
  imports: [FormField, FormRoot, RouterLink],
  selector: 'app-register',
  styleUrl: './register.scss',
  templateUrl: './register.html',
})
export class Register {
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly model = signal<RegisterForm>({ email: '', password: '', confirmPassword: '' });
  protected readonly showPassword = signal(false);
  protected readonly showConfirmPassword = signal(false);

  protected readonly registerForm = form(
    this.model, (path) => {
      required(path.email, { message: 'Saisissez votre adresse e-mail.'});
      email(path.email, { message: 'Saisissez une adresse e-mail valide, par exemple nom@exemple.fr.'});
      required(path.password, { message: 'Saisissez votre mot de passe.'});
      minLength(path.password, 12, { message: 'Le mot de passe doit contenir au moins 12 caractères.'});
      pattern(path.password, /[A-Z]/, { message: "Le mot de passe doit contenir au moins une lettre majuscule." });
      pattern(path.password, /[a-z]/, { message: "Le mot de passe doit contenir au moins une lettre minuscule." });
      pattern(path.password, /\d/, { message: "Le mot de passe doit contenir au moins un chiffre." });
      pattern(path.password, /[^A-Za-z0-9]/, { message: "Le mot de passe doit contenir au moins un caractère spécial, par exemple ! ou ?." });
      required(path.confirmPassword, { message: 'Confirmez votre mot de passe.' });
      validate(path.confirmPassword, (ctx) => 
        ctx.value() === ctx.valueOf(path.password)
        ? null
        : { kind: 'passwordMismatch', message: 'Les mots de passe ne correspondent pas.' }
    );
    },
    {
      submission: {
        action: async(field) => {
          const { email, password } = field().value(); // Avoid send confirmation to the API
          this.showPassword.set(false);
          this.showConfirmPassword.set(false);
          try {
            await this.auth.register({ email, password })
          } catch (error) {
            // 1- Identity codes errors : attached to field
            const fieldErrors = identityErrorCodes(error).flatMap((code) => {
              const entry = IDENTITY_ERRORS[code];
              return entry ? [{ kind: code, message: entry.message, fieldTree: field[entry.field]}] : [];
            });

            // 2- Same message has to be displayed only once (DuplicateUsername/DuplicateEmail)
            const uniqueErrors = fieldErrors.filter((error, index) => fieldErrors.findIndex((other) => other.message === error.message) === index);

            if (uniqueErrors.length > 0) {
              return uniqueErrors;
            }

            // 3- Not a specified error : return general error
            return { kind: 'serverError', message: "L'inscription a échoué. Réessayez dans quelques instants." }
          }
          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/accounts';
          await this.router.navigateByUrl(returnUrl);
          return undefined;
        },
        onInvalid: (field) => {
          field().errorSummary()[0]?.fieldTree().focusBoundControl();
        },
      },
    },
  );

  protected showErrors(field: FieldTree<string>): boolean {
    return field().touched() && field().invalid();
  }
}

// Send error code from Identity or none if unknown
function identityErrorCodes(error: unknown): string[] {
  if (!(error instanceof HttpErrorResponse) || error.status !== 400) {
    return [];
  }
  const body = error.error as ValidationProblemBody | null;
  return Object.keys(body?.errors ?? {});
}