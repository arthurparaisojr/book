import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { API_BASE_URL } from '../config/api.config';
import { Livro, LivroPayload, ListLivrosParams } from '../models/catalog.models';

@Injectable({ providedIn: 'root' })
export class LivrosApiService {
  private readonly http = inject(HttpClient);

  list(params: ListLivrosParams = {}) {
    let httpParams = new HttpParams();

    if (params.titulo) {
      httpParams = httpParams.set('titulo', params.titulo);
    }

    if (params.autorNome) {
      httpParams = httpParams.set('autorNome', params.autorNome);
    }

    if (params.assuntoDescricao) {
      httpParams = httpParams.set('assuntoDescricao', params.assuntoDescricao);
    }

    return this.http.get<Livro[]>(`${API_BASE_URL}/livros`, { params: httpParams });
  }

  create(payload: LivroPayload) {
    return this.http.post<Livro>(`${API_BASE_URL}/livros`, payload);
  }

  update(codl: number, payload: LivroPayload) {
    return this.http.put<Livro>(`${API_BASE_URL}/livros/${codl}`, payload);
  }

  delete(codl: number) {
    return this.http.delete<void>(`${API_BASE_URL}/livros/${codl}`);
  }
}
