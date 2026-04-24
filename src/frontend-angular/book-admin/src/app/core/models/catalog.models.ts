export interface Livro {
  codl: number;
  titulo: string;
  editora: string;
  edicao: number;
  anoPublicacao: string;
  valor: number;
}

export interface LivroPayload {
  titulo: string;
  editora: string;
  edicao: number;
  anoPublicacao: string;
  valor: number;
}

export interface ListLivrosParams {
  titulo?: string;
  autorNome?: string;
  assuntoDescricao?: string;
}

export interface Autor {
  codAu: number;
  nome: string;
}

export interface AutorPayload {
  nome: string;
}

export interface Assunto {
  codAs: number;
  descricao: string;
}

export interface AssuntoPayload {
  descricao: string;
}

export interface HealthCheckEntry {
  name: string;
  status: string;
  description: string;
}

export interface HealthResponse {
  service: string;
  status: string;
  utcNow: string;
  traceId: string;
  checks: HealthCheckEntry[];
}
