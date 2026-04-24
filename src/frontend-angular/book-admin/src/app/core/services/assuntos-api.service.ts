import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API_BASE_URL } from '../config/api.config';
import { Assunto, AssuntoPayload } from '../models/catalog.models';

@Injectable({ providedIn: 'root' })
export class AssuntosApiService {
  private readonly http = inject(HttpClient);

  list(descricao = '') {
    const params = descricao ? new HttpParams().set('descricao', descricao) : undefined;
    return this.http.get<Assunto[]>(`${API_BASE_URL}/assuntos`, { params });
  }

  create(payload: AssuntoPayload) {
    return this.http.post<Assunto>(`${API_BASE_URL}/assuntos`, payload);
  }

  update(codAs: number, payload: AssuntoPayload) {
    return this.http.put<Assunto>(`${API_BASE_URL}/assuntos/${codAs}`, payload);
  }

  delete(codAs: number) {
    return this.http.delete<void>(`${API_BASE_URL}/assuntos/${codAs}`);
  }
}
