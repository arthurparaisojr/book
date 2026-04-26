import { AbstractControl } from '@angular/forms';

export interface ValidationMessageMap {
  required?: string;
  maxlength?: string;
  minlength?: string;
  min?: string;
  max?: string;
  pattern?: string;
  currencyFormat?: string;
}

export function shouldShowControlError(control: AbstractControl | null): boolean {
  return !!control && control.invalid && (control.touched || control.dirty);
}

export function getControlErrorMessage(
  control: AbstractControl | null,
  messages: ValidationMessageMap
): string {
  if (!shouldShowControlError(control) || !control?.errors) {
    return '';
  }

  if (control.errors['required']) {
    return messages.required ?? 'Campo obrigatorio.';
  }

  if (control.errors['maxlength']) {
    const requiredLength = control.errors['maxlength']['requiredLength'];
    return messages.maxlength ?? `Informe no maximo ${requiredLength} caracteres.`;
  }

  if (control.errors['minlength']) {
    const requiredLength = control.errors['minlength']['requiredLength'];
    return messages.minlength ?? `Informe ao menos ${requiredLength} caracteres.`;
  }

  if (control.errors['min']) {
    const minimum = control.errors['min']['min'];
    return messages.min ?? `Informe um valor maior ou igual a ${minimum}.`;
  }

  if (control.errors['max']) {
    const maximum = control.errors['max']['max'];
    return messages.max ?? `Informe um valor menor ou igual a ${maximum}.`;
  }

  if (control.errors['pattern']) {
    return messages.pattern ?? 'Formato invalido.';
  }

  if (control.errors['currencyFormat']) {
    return messages.currencyFormat ?? 'Informe um valor monetario valido.';
  }

  return 'Revise o valor informado.';
}
