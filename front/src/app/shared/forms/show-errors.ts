import { FieldTree } from "@angular/forms/signals";

export function showErrors<T>(field: FieldTree<T>): boolean {
  return field().touched() && field().invalid();
}
