import { AfterViewInit, Directive, ElementRef, HostListener, inject } from '@angular/core';
import { NgControl } from '@angular/forms';
import { formatBrlInputValue } from '../../core/utils/currency.utils';

@Directive({
  selector: 'input[appCurrencyMask]',
  standalone: true
})
export class CurrencyMaskDirective implements AfterViewInit {
  private readonly elementRef = inject(ElementRef<HTMLInputElement>);
  private readonly ngControl = inject(NgControl, { optional: true, self: true });

  ngAfterViewInit(): void {
    this.applyMask(this.elementRef.nativeElement.value, false);
  }

  @HostListener('input', ['$event.target.value'])
  onInput(value: string): void {
    this.applyMask(value, false);
  }

  @HostListener('blur')
  onBlur(): void {
    this.applyMask(this.elementRef.nativeElement.value, true);
  }

  private applyMask(rawValue: string, markAsTouched: boolean): void {
    const digitsOnly = rawValue.replace(/\D/g, '');
    const formattedValue = digitsOnly
      ? formatBrlInputValue(Number(digitsOnly) / 100)
      : '';

    this.elementRef.nativeElement.value = formattedValue;

    const control = this.ngControl?.control;

    if (control) {
      control.setValue(formattedValue, { emitEvent: false });
      control.updateValueAndValidity({ emitEvent: false });

      if (markAsTouched) {
        control.markAsTouched();
      }
    }
  }
}
