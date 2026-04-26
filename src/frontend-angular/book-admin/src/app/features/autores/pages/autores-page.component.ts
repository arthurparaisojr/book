import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Autor, AutorPayload } from '../../../core/models/catalog.models';
import { AutoresApiService } from '../../../core/services/autores-api.service';
import { getApiErrorMessage } from '../../../core/utils/api-error.utils';
import {
  ValidationMessageMap,
  getControlErrorMessage,
  shouldShowControlError
} from '../../../core/utils/form-error.utils';

@Component({
  selector: 'app-autores-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './autores-page.component.html',
  styleUrl: './autores-page.component.css'
})
export class AutoresPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly autoresApiService = inject(AutoresApiService);

  readonly filterForm = this.formBuilder.nonNullable.group({
    nome: ['']
  });

  readonly autorForm = this.formBuilder.nonNullable.group({
    nome: ['', [Validators.required, Validators.maxLength(40)]]
  });

  autores: Autor[] = [];
  editingAutorId: number | null = null;
  loading = true;
  saving = false;
  errorMessage = '';
  successMessage = '';
  readonly editIconPath = 'icons/book-action-editar.svg';
  readonly deleteIconPath = 'icons/book-action-excluir.svg';
  private readonly validationMessages: Record<string, ValidationMessageMap> = {
    nome: {
      required: 'Informe o nome do autor.',
      maxlength: 'Use ate 40 caracteres para o nome do autor.'
    }
  };

  ngOnInit(): void {
    this.loadAutores();
  }

  loadAutores(): void {
    this.loading = true;
    this.errorMessage = '';

    this.autoresApiService.list(this.filterForm.controls.nome.value).subscribe({
      next: (autores) => {
        this.autores = autores;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = getApiErrorMessage(error);
        this.loading = false;
      }
    });
  }

  clearFilters(): void {
    this.filterForm.reset({ nome: '' });
    this.loadAutores();
  }

  startCreate(): void {
    this.editingAutorId = null;
    this.successMessage = '';
    this.autorForm.reset({ nome: '' });
  }

  startEdit(autor: Autor): void {
    this.editingAutorId = autor.codAu;
    this.successMessage = '';
    this.autorForm.setValue({ nome: autor.nome });
  }

  submit(): void {
    if (this.autorForm.invalid) {
      this.autorForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const payload: AutorPayload = { nome: this.autorForm.controls.nome.value.trim() };
    const request$ = this.editingAutorId
      ? this.autoresApiService.update(this.editingAutorId, payload)
      : this.autoresApiService.create(payload);

    request$.subscribe({
      next: () => {
        this.saving = false;
        this.successMessage = this.editingAutorId
          ? 'Autor atualizado com sucesso.'
          : 'Autor criado com sucesso.';
        this.startCreate();
        this.loadAutores();
      },
      error: (error) => {
        this.saving = false;
        this.errorMessage = getApiErrorMessage(error);
      }
    });
  }

  remove(autor: Autor): void {
    const shouldDelete = window.confirm(`Deseja excluir o autor "${autor.nome}"?`);

    if (!shouldDelete) {
      return;
    }

    this.autoresApiService.delete(autor.codAu).subscribe({
      next: () => {
        this.successMessage = 'Autor excluido com sucesso.';
        if (this.editingAutorId === autor.codAu) {
          this.startCreate();
        }

        this.loadAutores();
      },
      error: (error) => {
        this.errorMessage = getApiErrorMessage(error);
      }
    });
  }

  showFieldError(controlName: 'nome'): boolean {
    return shouldShowControlError(this.autorForm.controls[controlName]);
  }

  getFieldErrorMessage(controlName: 'nome'): string {
    return getControlErrorMessage(
      this.autorForm.controls[controlName],
      this.validationMessages[controlName]
    );
  }
}
