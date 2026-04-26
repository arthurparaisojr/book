import { CommonModule } from '@angular/common';
import { Component, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { getApiErrorMessage } from '../../../core/utils/api-error.utils';
import {
  ValidationMessageMap,
  getControlErrorMessage,
  shouldShowControlError
} from '../../../core/utils/form-error.utils';

@Component({
  selector: 'app-login-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './login-page.component.html',
  styleUrl: './login-page.component.css'
})
export class LoginPageComponent {
  private readonly formBuilder = inject(FormBuilder);
  private readonly authService = inject(AuthService);
  private readonly router = inject(Router);

  readonly loginForm = this.formBuilder.nonNullable.group({
    username: ['', [Validators.required]],
    password: ['', [Validators.required]]
  });

  isSubmitting = false;
  errorMessage = '';
  readonly currentYear = new Date().getFullYear();
  private readonly validationMessages: Record<string, ValidationMessageMap> = {
    username: {
      required: 'Informe o usuario para continuar.'
    },
    password: {
      required: 'Informe a senha para autenticar.'
    }
  };

  useCredentialPreset(username: string, password: string): void {
    this.loginForm.setValue({ username, password });
    this.errorMessage = '';
  }

  submit(): void {
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.errorMessage = '';

    this.authService.login(this.loginForm.getRawValue()).subscribe({
      next: () => {
        this.isSubmitting = false;
        void this.router.navigateByUrl('/dashboard');
      },
      error: (error) => {
        this.isSubmitting = false;
        this.errorMessage = getApiErrorMessage(error);
      }
    });
  }

  showFieldError(controlName: 'username' | 'password'): boolean {
    return shouldShowControlError(this.loginForm.controls[controlName]);
  }

  getFieldErrorMessage(controlName: 'username' | 'password'): string {
    return getControlErrorMessage(
      this.loginForm.controls[controlName],
      this.validationMessages[controlName]
    );
  }
}
