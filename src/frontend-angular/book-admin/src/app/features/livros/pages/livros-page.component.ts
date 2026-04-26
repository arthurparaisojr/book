import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Livro, LivroPayload } from '../../../core/models/catalog.models';
import { LivrosApiService } from '../../../core/services/livros-api.service';
import { getApiErrorMessage } from '../../../core/utils/api-error.utils';
import {
  brlCurrencyInputValidator,
  formatBrlInputValue,
  parseBrlInputValue
} from '../../../core/utils/currency.utils';
import {
  ValidationMessageMap,
  getControlErrorMessage,
  shouldShowControlError
} from '../../../core/utils/form-error.utils';
import { CurrencyMaskDirective } from '../../../shared/directives/currency-mask.directive';

@Component({
  selector: 'app-livros-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, CurrencyMaskDirective],
  templateUrl: './livros-page.component.html',
  styleUrl: './livros-page.component.css'
})
export class LivrosPageComponent implements OnInit {
  private readonly formBuilder = inject(FormBuilder);
  private readonly livrosApiService = inject(LivrosApiService);

  readonly filterForm = this.formBuilder.nonNullable.group({
    titulo: [''],
    autorNome: [''],
    assuntoDescricao: ['']
  });

  readonly livroForm = this.formBuilder.nonNullable.group({
    titulo: ['', [Validators.required, Validators.maxLength(40)]],
    editora: ['', [Validators.required, Validators.maxLength(40)]],
    edicao: [1, [Validators.required, Validators.min(1)]],
    anoPublicacao: [
      new Date().getFullYear().toString(),
      [Validators.required, Validators.pattern(/^\d{4}$/)]
    ],
    valor: ['', [Validators.required, brlCurrencyInputValidator()]]
  });

  livros: Livro[] = [];
  editingLivroId: number | null = null;
  loading = true;
  saving = false;
  errorMessage = '';
  successMessage = '';
  readonly editIconPath = 'icons/book-action-editar.svg';
  readonly deleteIconPath = 'icons/book-action-excluir.svg';
  private readonly validationMessages: Record<string, ValidationMessageMap> = {
    titulo: {
      required: 'Informe o titulo do livro.',
      maxlength: 'Use ate 40 caracteres no titulo.'
    },
    editora: {
      required: 'Informe a editora do livro.',
      maxlength: 'Use ate 40 caracteres no nome da editora.'
    },
    edicao: {
      required: 'Informe a edicao.',
      min: 'A edicao deve ser maior ou igual a 1.'
    },
    anoPublicacao: {
      required: 'Informe o ano de publicacao.',
      pattern: 'Informe o ano com 4 digitos, por exemplo 2026.'
    },
    valor: {
      required: 'Informe o valor do livro.',
      currencyFormat: 'Digite o valor no padrao monetario brasileiro, por exemplo 129,90.'
    }
  };

  ngOnInit(): void {
    this.loadLivros();
  }

  loadLivros(): void {
    this.loading = true;
    this.errorMessage = '';

    this.livrosApiService.list(this.filterForm.getRawValue()).subscribe({
      next: (livros) => {
        this.livros = livros;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = getApiErrorMessage(error);
        this.loading = false;
      }
    });
  }

  clearFilters(): void {
    this.filterForm.reset({
      titulo: '',
      autorNome: '',
      assuntoDescricao: ''
    });
    this.loadLivros();
  }

  startCreate(): void {
    this.editingLivroId = null;
    this.successMessage = '';
    this.livroForm.reset({
      titulo: '',
      editora: '',
      edicao: 1,
      anoPublicacao: new Date().getFullYear().toString(),
      valor: ''
    });
  }

  startEdit(livro: Livro): void {
    this.editingLivroId = livro.codl;
    this.successMessage = '';
    this.livroForm.setValue({
      titulo: livro.titulo,
      editora: livro.editora,
      edicao: livro.edicao,
      anoPublicacao: livro.anoPublicacao,
      valor: formatBrlInputValue(livro.valor)
    });
  }

  cancelEdit(): void {
    this.startCreate();
  }

  submit(): void {
    if (this.livroForm.invalid) {
      this.livroForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.errorMessage = '';
    this.successMessage = '';

    const payload = this.buildPayload();
    const request$ = this.editingLivroId
      ? this.livrosApiService.update(this.editingLivroId, payload)
      : this.livrosApiService.create(payload);

    request$.subscribe({
      next: () => {
        this.saving = false;
        this.successMessage = this.editingLivroId
          ? 'Livro atualizado com sucesso.'
          : 'Livro criado com sucesso.';
        this.startCreate();
        this.loadLivros();
      },
      error: (error) => {
        this.saving = false;
        this.errorMessage = getApiErrorMessage(error);
      }
    });
  }

  remove(livro: Livro): void {
    const shouldDelete = window.confirm(`Deseja excluir o livro "${livro.titulo}"?`);

    if (!shouldDelete) {
      return;
    }

    this.errorMessage = '';
    this.successMessage = '';

    this.livrosApiService.delete(livro.codl).subscribe({
      next: () => {
        this.successMessage = 'Livro excluido com sucesso.';
        if (this.editingLivroId === livro.codl) {
          this.startCreate();
        }

        this.loadLivros();
      },
      error: (error) => {
        this.errorMessage = getApiErrorMessage(error);
      }
    });
  }

  formatCurrency(value: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL'
    }).format(value);
  }

  showFieldError(
    controlName: 'titulo' | 'editora' | 'edicao' | 'anoPublicacao' | 'valor'
  ): boolean {
    return shouldShowControlError(this.livroForm.controls[controlName]);
  }

  getFieldErrorMessage(
    controlName: 'titulo' | 'editora' | 'edicao' | 'anoPublicacao' | 'valor'
  ): string {
    return getControlErrorMessage(
      this.livroForm.controls[controlName],
      this.validationMessages[controlName]
    );
  }

  private buildPayload(): LivroPayload {
    const value = this.livroForm.getRawValue();

    return {
      titulo: value.titulo.trim(),
      editora: value.editora.trim(),
      edicao: Number(value.edicao),
      anoPublicacao: value.anoPublicacao.trim(),
      valor: parseBrlInputValue(value.valor) ?? 0
    };
  }
}
