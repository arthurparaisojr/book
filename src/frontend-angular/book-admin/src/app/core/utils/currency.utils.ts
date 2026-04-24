import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const decimalFormatter = new Intl.NumberFormat('pt-BR', {
  minimumFractionDigits: 2,
  maximumFractionDigits: 2
});

export function formatBrlInputValue(value: number | null | undefined): string {
  if (value === null || value === undefined || Number.isNaN(value)) {
    return '';
  }

  return decimalFormatter.format(value);
}

export function parseBrlInputValue(value: string | number | null | undefined): number | null {
  if (value === null || value === undefined || value === '') {
    return null;
  }

  if (typeof value === 'number') {
    return Number.isFinite(value) ? value : null;
  }

  const normalizedValue = value
    .replace(/\s/g, '')
    .replace(/\./g, '')
    .replace(',', '.')
    .replace(/[^0-9.-]/g, '');

  if (!normalizedValue) {
    return null;
  }

  const parsedValue = Number.parseFloat(normalizedValue);

  return Number.isFinite(parsedValue) ? parsedValue : null;
}

export function brlCurrencyInputValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    if (control.value === null || control.value === undefined || control.value === '') {
      return null;
    }

    const parsedValue = parseBrlInputValue(control.value);

    if (parsedValue === null) {
      return { currencyFormat: true };
    }

    if (parsedValue < 0) {
      return {
        min: {
          min: 0,
          actual: parsedValue
        }
      };
    }

    return null;
  };
}
