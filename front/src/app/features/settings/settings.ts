import { Component, inject, signal } from '@angular/core';
import { form, validate, FormField, FormRoot } from '@angular/forms/signals';
import { Auth } from '../../core/auth/auth';
import { Router } from '@angular/router';
import { LiveAnnouncer } from '@angular/cdk/a11y';
import { showErrors } from '../../shared/forms/show-errors';

@Component({
  imports: [FormField, FormRoot],
  selector: 'app-settings',
  styleUrl: './settings.scss',
  templateUrl: './settings.html',
})
export class Settings {
  private readonly auth = inject(Auth);
  private readonly router = inject(Router);
  private readonly liveAnnouncer = inject(LiveAnnouncer);

  protected readonly model = signal({ confirmed: false });
  protected readonly showErrors = showErrors;

  protected readonly deleteAccountForm = form(
    this.model, (path) => {
      validate(path.confirmed, (ctx) =>
        ctx.value() ? undefined : { kind: 'notConfirmed', message: 'Cochez la case pour confirmer la suppression.'},
      );
    },
    {
      submission: {
        action: async() => {
          try {
            await this.auth.deleteAccount();
          } catch {
            return {
              kind: 'serverError',
              message: 'La suppression du compte a échoué. Réessayez dans quelques instants.',
            };
          }
          await this.router.navigate(['/']);
          await this.liveAnnouncer.announce('Votre compte a été supprimé.');
          return undefined;
        },
        onInvalid: (field) => {
          field().errorSummary()[0]?.fieldTree().focusBoundControl();
        },
      }
    }
  )
}
