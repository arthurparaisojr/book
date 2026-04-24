import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Livro, LivroPayload } from '../../../core/models/catalog.models';
import { LivrosApiService } from '../../../core/services/livros-api.service';
import { getApiErrorMessage } from '../../../core/utils/api-error.utils';

@Component({
  selector: 'app-livros-page',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
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
    anoPublicacao: [new Date().getFullYear().toString(), [Validators.required, Validators.minLength(4), Validators.maxLength(4)]],
    valor: [0, [Validators.required, Validators.min(0)]]
  });

  livros: Livro[] = [];
  editingLivroId: number | null = null;
  loading = true;
  saving = false;
  errorMessage = '';
  successMessage = '';

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
      valor: 0
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
      valor: livro.valor
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

  private buildPayload(): LivroPayload {
    const value = this.livroForm.getRawValue();

    return {
      titulo: value.titulo.trim(),
      editora: value.editora.trim(),
      edicao: Number(value.edicao),
      anoPublicacao: value.anoPublicacao.trim(),
      valor: Number(value.valor)
    };
  }
}
