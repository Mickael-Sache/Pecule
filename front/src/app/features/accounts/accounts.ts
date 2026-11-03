import { HttpClient, httpResource } from '@angular/common/http';
import { Component, inject, signal } from '@angular/core';
import type { components } from '../../api/schema';
import { CurrencyPipe } from '@angular/common';
import { form, maxLength, required, FormField, FormRoot } from '@angular/forms/signals';
import { firstValueFrom } from 'rxjs';
import { showErrors } from '../../shared/forms/show-errors';
import { validationErrors } from '../../shared/forms/validation-errors';
import { LiveAnnouncer } from '@angular/cdk/a11y';

type AccountDto = components['schemas']['AccountDto'];
type AccountType = AccountDto['type'];
type CreateAccountRequest = components['schemas']['CreateAccountRequest']

const ACCOUNT_TYPE_LABELS: Record<AccountType, string> = {
  Checking: 'Compte courant',
  Savings: 'Livret',
};

const SERVER_FIELDS: Partial<Record<string, keyof CreateAccountRequest>> = {
  Name: 'name',
  Type: 'type',
  InitialBalance: 'initialBalance',
};

@Component({
  imports: [CurrencyPipe, FormField, FormRoot],
  selector: 'app-accounts',
  styleUrl: './accounts.scss',
  templateUrl: './accounts.html',
})
export class Accounts {
  private readonly http = inject(HttpClient);
  private readonly announcer = inject(LiveAnnouncer);

  protected readonly accounts = httpResource<AccountDto[]>(() => '/api/accounts');
  protected readonly model = signal<CreateAccountRequest>({ name: '', type: 'Checking', initialBalance: 0});
  protected readonly typeLabels = ACCOUNT_TYPE_LABELS;
  protected readonly typeOptions = Object.entries(ACCOUNT_TYPE_LABELS) as [AccountType, string][];
  protected readonly showErrors = showErrors;

  protected readonly accountForm = form(
    this.model, (path) => {
      required(path.name, { message: 'Saisissez un nom de compte.'});
      maxLength(path.name, 60, { message: 'Le nom de compte ne peut excéder 60 caractères.' });
    },
    {
      submission: {
        action: async(field) => {
          try {
            await firstValueFrom(this.http.post<AccountDto>('/api/accounts', field().value()));
          } catch (error) {
            const errors = Object.entries(validationErrors(error)).flatMap(([key, messages]) => {
              const name = SERVER_FIELDS[key];
              return name ? messages.map((messages) => ({ kind: 'server', messages, fieldTree: field[name] })) : [];
            });
            return errors.length > 0
            ? errors
            : { kind: 'serverError', message: 'Impossible de créer le compte. Réessayer.' };
          }
          this.accounts.reload();
          await this.announcer.announce('Compte créé.');
          return undefined;
        },
        onInvalid: (field) => {
          field().errorSummary()[0]?.fieldTree().focusBoundControl();
        }
      }
    }
  )
}
