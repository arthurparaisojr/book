import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Assunto, AssuntoPayload } from '../../../core/models/catalog.models';
import { AssuntosApiService } from '../../../core/services/assuntos-api.service';
import { getApiErrorMessage } from '../../../core/utils/api-error.utils';
import {
  ValidationMessageMap,
  getControlErrorMessage,
  shouldShowControlError
} from '../../../core/utils/form-error.utils';

@Component({
  selector: 'app-assuntos-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './assuntos-page.component.html',
  styleUrl: './assuntos-page.component.css'
})
export class AssuntosPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly assuntosApiService = inject(AssuntosApiService);

  readonly filterForm = this.formBuilder.nonNullable.group({
    descricao: ['']
  });

  readonly assuntoForm = this.formBuilder.nonNullable.group({
    descricao: ['', [Validators.required, Validators.maxLength(20)]]
  });

  assuntos: Assunto[] = [];
  editingAssuntoId: number | null = null;
  loading = true;
  saving = false;
  errorMessage = '';
  successMessage = '';
  readonly editIconPath = 'icons/book-action-editar.svg';
  readonly deleteIconPath = 'icons/book-action-excluir.svg';
  private readonly validationMessages: Record<string, ValidationMessageMap> = {
    descricao: {
      required: 'Informe a descricao do assunto.',
      maxlength: 'Use ate 20 caracteres para a descricao.'
    }
  };

  ngOnInit(): void {
    this.loadAssuntos();
  }

  loadAssuntos(): void {
    this.loading = true;
    this.errorMessage = '';

    this.assuntosApiService.list(this.filterForm.controls.descricao.value).subscribe({
      next: (assuntos) => {
        this.assuntos = assuntos;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = getApiErrorMessage(error);
        this.loading = false;
      }
    });
  }

  clearFilters(): void {
    this.filterForm.reset({ descricao: '' });
    this.loadAssuntos();
  }

  startCreate(): void {
    this.editingAssuntoId = null;
    this.successMessage = '';
    this.assuntoForm.reset({ descricao: '' });
  }

  startEdit(assunto: Assunto): void {
    this.editingAssuntoId = assunto.codAs;
    this.successMessage = '';
    this.assuntoForm.setValue({ descricao: assunto.descricao });
  }

  submit(): void {
    if (this.assuntoForm.invalid) {
      this.assuntoForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const payload: AssuntoPayload = {
      descricao: this.assuntoForm.controls.descricao.value.trim()
    };
    const request$ = this.editingAssuntoId
      ? this.assuntosApiService.update(this.editingAssuntoId, payload)
      : this.assuntosApiService.create(payload);

    request$.subscribe({
      next: () => {
        this.saving = false;
        this.successMessage = this.editingAssuntoId
          ? 'Assunto atualizado com sucesso.'
          : 'Assunto criado com sucesso.';
        this.startCreate();
        this.loadAssuntos();
      },
      error: (error) => {
        this.saving = false;
        this.errorMessage = getApiErrorMessage(error);
      }
    });
  }

  remove(assunto: Assunto): void {
    const shouldDelete = window.confirm(`Deseja excluir o assunto "${assunto.descricao}"?`);

    if (!shouldDelete) {
      return;
    }

    this.assuntosApiService.delete(assunto.codAs).subscribe({
      next: () => {
        this.successMessage = 'Assunto excluido com sucesso.';
        if (this.editingAssuntoId === assunto.codAs) {
          this.startCreate();
        }

        this.loadAssuntos();
      },
      error: (error) => {
        this.errorMessage = getApiErrorMessage(error);
      }
    });
  }

  showFieldError(controlName: 'descricao'): boolean {
    return shouldShowControlError(this.assuntoForm.controls[controlName]);
  }

  getFieldErrorMessage(controlName: 'descricao'): string {
    return getControlErrorMessage(
      this.assuntoForm.controls[controlName],
      this.validationMessages[controlName]
    );
  }
}
