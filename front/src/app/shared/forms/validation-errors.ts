import { HttpErrorResponse } from "@angular/common/http";

interface ValidationProblemBody {
  errors?: Record<string, string[]>;
}

/*
 * Function to read errors from ValidationProblem (400) send by API
 * Return: Empty object for other error : other status, API unreachable, JavaScript error
*/
export function validationErrors(error: unknown): Record<string, string[]> {
  if (!(error instanceof HttpErrorResponse) || error.status !== 400) {
    return {};
  }
  const body = error.error as ValidationProblemBody | null;
  return body?.errors ?? {};
}
