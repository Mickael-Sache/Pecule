import { HttpErrorResponse } from '@angular/common/http';
import { validationErrors } from './validation-errors';

describe('validationErrors', () => {
  it('returns the errors of a 400 validation problem', () => {
    const error = new HttpErrorResponse({
      status: 400,
      error: { errors: { Name: ['Le nom est obligatoire.'] } },
    });

    expect(validationErrors(error)).toEqual({ Name: ['Le nom est obligatoire.'] });
  });

  it('returns an empty object for another status', () => {
    const error = new HttpErrorResponse({
      status: 500,
      error: { errors: { Name: ['Le nom est obligatoire.'] } },
    });

    expect(validationErrors(error)).toEqual({});
  });

  it('returns an empty object for a 400 without errors', () => {
    const error = new HttpErrorResponse({ status: 400, error: { title: 'Bad Request' } });

    expect(validationErrors(error)).toEqual({});
  });

  it('returns an empty object for a non-HTTP error', () => {
    expect(validationErrors(new Error('boom'))).toEqual({});
  });
});
