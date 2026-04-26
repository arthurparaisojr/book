import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { API_BASE_URL } from '../config/api.config';
import { HealthResponse } from '../models/catalog.models';

@Injectable({ providedIn: 'root' })
export class HealthApiService {
  private readonly http = inject(HttpClient);

  getHealth() {
    return this.http.get<HealthResponse>(`${API_BASE_URL}/health`);
  }

  getLive() {
    return this.http.get<HealthResponse>(`${API_BASE_URL}/health/live`);
  }

  getReady() {
    return this.http.get<HealthResponse>(`${API_BASE_URL}/health/ready`);
  }
}
