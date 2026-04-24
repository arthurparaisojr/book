import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { HealthApiService } from '../../../core/services/health-api.service';

interface NavigationItem {
  label: string;
  route: string;
  description: string;
  iconPath?: string;
  shortLabel: string;
}

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './admin-layout.component.html',
  styleUrl: './admin-layout.component.css'
})
export class AdminLayoutComponent implements OnInit {
  private readonly router = inject(Router);
  private readonly authService = inject(AuthService);
  private readonly healthApiService = inject(HealthApiService);

  readonly navigationItems: NavigationItem[] = [
    {
      label: 'Dashboard',
      route: '/dashboard',
      description: 'Visao geral do ambiente',
      iconPath: 'icons/book-nav-dashboard.svg',
      shortLabel: 'DB'
    },
    {
      label: 'Livros',
      route: '/livros',
      description: 'Cadastro principal',
      iconPath: 'icons/book-nav-livros.svg',
      shortLabel: 'LV'
    },
    {
      label: 'Autores',
      route: '/autores',
      description: 'Controle de autores',
      iconPath: 'icons/book-nav-autores.svg',
      shortLabel: 'AU'
    },
    {
      label: 'Assuntos',
      route: '/assuntos',
      description: 'Classificacao tematica',
      iconPath: 'icons/book-nav-assuntos.svg',
      shortLabel: 'AS'
    }
  ];

  apiStatus = 'Verificando API';
  apiStatusClass = 'book-state-warning';

  ngOnInit(): void {
    this.refreshHealthStatus();
  }

  get username(): string {
    return this.authService.username();
  }

  get role(): string {
    return this.authService.role();
  }

  logout(): void {
    this.authService.logout();
    void this.router.navigateByUrl('/login');
  }

  refreshHealthStatus(): void {
    this.healthApiService.getReady().subscribe({
      next: () => {
        this.apiStatus = 'API pronta';
        this.apiStatusClass = 'book-state-success';
      },
      error: () => {
        this.apiStatus = 'API indisponivel';
        this.apiStatusClass = 'book-state-danger';
      }
    });
  }
}
