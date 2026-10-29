import { Component, inject, signal } from '@angular/core';
import { email, FieldTree, form, FormField, FormRoot, required } from '@angular/forms/signals';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Auth, Credentials } from '../../core/auth/auth';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  imports: [FormField, FormRoot, RouterLink],
  selector: 'app-login',
  styleUrl: './login.scss',
  templateUrl: './login.html',
})
export class Login {
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  protected readonly model = signal<Credentials>({ email: '', password: '' });

  protected readonly loginForm = form(
    this.model, (path) => {
      required(path.email,  { message: 'Saisissez votre adresse e-mail.'});
      email(path.email, { message: 'Saisissez une adresse e-mail valide, par exemple nom@exemple.fr.' });
      required(path.password, { message: 'Saisissez votre mot de passe.'});
    },
    {
      submission: {
        action: async (field) => {
          try {
            await this.auth.login(field().value());
          } catch (error) {
            if (error instanceof HttpErrorResponse && error.status === 401) {
              return { kind: 'invalidCredentials', message: 'Adresse e-mail ou mot de passe incorrect.' };
            }
            return { kind: 'serverError', message: 'La connexion a échoué. Réessayer dans quelques instants.' };
          }
          const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl') ?? '/accounts';
          await this.router.navigateByUrl(returnUrl);
          return undefined;
        },
        // Form is invalid : focus on first field on error
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
