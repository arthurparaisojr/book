import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { tap } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';
import { AuthSession, AuthTokenResponse, LoginRequest } from '../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly storageKey = 'book-admin-session';
  private readonly sessionState = signal<AuthSession | null>(this.readSession());

  readonly session = computed(() => this.sessionState());
  readonly isAuthenticated = computed(() => this.sessionState() !== null);
  readonly username = computed(() => this.sessionState()?.username ?? 'Convidado');
  readonly role = computed(() => this.sessionState()?.role ?? 'Anonimo');

  login(request: LoginRequest) {
    return this.http.post<AuthTokenResponse>(`${API_BASE_URL}/auth/login`, request).pipe(
      tap((response) =>
        this.setSession({
          accessToken: response.accessToken,
          expiresAtUtc: response.expiresAtUtc,
          username: response.username,
          role: response.role
        })
      )
    );
  }

  logout(): void {
    localStorage.removeItem(this.storageKey);
    this.sessionState.set(null);
  }

  getAccessToken(): string | null {
    return this.sessionState()?.accessToken ?? null;
  }

  private setSession(session: AuthSession): void {
    localStorage.setItem(this.storageKey, JSON.stringify(session));
    this.sessionState.set(session);
  }

  private readSession(): AuthSession | null {
    const rawSession = localStorage.getItem(this.storageKey);

    if (!rawSession) {
      return null;
    }

    try {
      return JSON.parse(rawSession) as AuthSession;
    } catch {
      localStorage.removeItem(this.storageKey);
      return null;
    }
  }
}
