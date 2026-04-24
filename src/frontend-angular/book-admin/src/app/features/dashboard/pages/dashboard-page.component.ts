import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { forkJoin } from 'rxjs';
import { AutoresApiService } from '../../../core/services/autores-api.service';
import { AssuntosApiService } from '../../../core/services/assuntos-api.service';
import { HealthApiService } from '../../../core/services/health-api.service';
import { LivrosApiService } from '../../../core/services/livros-api.service';
import { getApiErrorMessage } from '../../../core/utils/api-error.utils';

@Component({
  selector: 'app-dashboard-page',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard-page.component.html',
  styleUrl: './dashboard-page.component.css'
})
export class DashboardPageComponent implements OnInit {
  private readonly livrosApiService = inject(LivrosApiService);
  private readonly autoresApiService = inject(AutoresApiService);
  private readonly assuntosApiService = inject(AssuntosApiService);
  private readonly healthApiService = inject(HealthApiService);

  loading = true;
  errorMessage = '';
  cards = [
    { label: 'Livros', value: 0, hint: 'Cadastros principais em operacao' },
    { label: 'Autores', value: 0, hint: 'Base de autoria disponivel' },
    { label: 'Assuntos', value: 0, hint: 'Classificacao para filtros e relatorios' }
  ];
  readinessStatus = 'Verificando';

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;
    this.errorMessage = '';

    forkJoin({
      livros: this.livrosApiService.list(),
      autores: this.autoresApiService.list(),
      assuntos: this.assuntosApiService.list(),
      ready: this.healthApiService.getReady()
    }).subscribe({
      next: ({ livros, autores, assuntos, ready }) => {
        this.cards = [
          { label: 'Livros', value: livros.length, hint: 'Cadastros principais em operacao' },
          { label: 'Autores', value: autores.length, hint: 'Base de autoria disponivel' },
          { label: 'Assuntos', value: assuntos.length, hint: 'Classificacao para filtros e relatorios' }
        ];
        this.readinessStatus = ready.status;
        this.loading = false;
      },
      error: (error) => {
        this.errorMessage = getApiErrorMessage(error);
        this.loading = false;
      }
    });
  }
}
