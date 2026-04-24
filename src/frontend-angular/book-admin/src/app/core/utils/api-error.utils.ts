import { HttpErrorResponse } from '@angular/common/http';

interface ProblemDetailsResponse {
  detail?: string;
  title?: string;
  errors?: Record<string, string[]>;
}

export function getApiErrorMessage(error: unknown): string {
  if (error instanceof HttpErrorResponse) {
    const body = error.error as ProblemDetailsResponse | string | null;

    if (typeof body === 'string' && body.trim().length > 0) {
      return body;
    }

    if (body && typeof body === 'object') {
      if (body.detail) {
        return body.detail;
      }

      if (body.errors) {
        const firstError = Object.values(body.errors).flat()[0];
        if (firstError) {
          return firstError;
        }
      }

      if (body.title) {
        return body.title;
      }
    }

    if (error.status === 0) {
      return 'Nao foi possivel acessar a API. Verifique se o backend esta ativo em http://localhost:5268.';
    }

    return `A requisicao falhou com status ${error.status}.`;
  }

  return 'Ocorreu um erro inesperado.';
}
