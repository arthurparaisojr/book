import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API_BASE_URL } from '../config/api.config';
import { Autor, AutorPayload } from '../models/catalog.models';

@Injectable({ providedIn: 'root' })
export class AutoresApiService {
  private readonly http = inject(HttpClient);

  list(nome = '') {
    const params = nome ? new HttpParams().set('nome', nome) : undefined;
    return this.http.get<Autor[]>(`${API_BASE_URL}/autores`, { params });
  }

  create(payload: AutorPayload) {
    return this.http.post<Autor>(`${API_BASE_URL}/autores`, payload);
  }

  update(codAu: number, payload: AutorPayload) {
    return this.http.put<Autor>(`${API_BASE_URL}/autores/${codAu}`, payload);
  }

  delete(codAu: number) {
    return this.http.delete<void>(`${API_BASE_URL}/autores/${codAu}`);
  }
}
