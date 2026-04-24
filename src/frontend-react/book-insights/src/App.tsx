import { useDeferredValue, useEffect, useState, startTransition } from 'react'
import dashboardIcon from '../../../shared/icons/svg/book-nav-dashboard.svg'
import livrosIcon from '../../../shared/icons/svg/book-nav-livros.svg'
import autoresIcon from '../../../shared/icons/svg/book-nav-autores.svg'
import assuntosIcon from '../../../shared/icons/svg/book-nav-assuntos.svg'
import './App.css'

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5268/api/v1'

interface Livro {
  codl: number
  titulo: string
  editora: string
  edicao: number
  anoPublicacao: string
  valor: number
}

interface Autor {
  codAu: number
  nome: string
}

interface Assunto {
  codAs: number
  descricao: string
}

interface HealthCheckEntry {
  name: string
  status: string
  description: string
}

interface HealthResponse {
  service: string
  status: string
  utcNow: string
  traceId: string
  checks: HealthCheckEntry[]
}

interface InsightSnapshot {
  health: HealthResponse
  livros: Livro[]
  autores: Autor[]
  assuntos: Assunto[]
}

type StatusTone = 'book-state-success' | 'book-state-warning' | 'book-state-danger'

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

async function fetchJson<T>(path: string): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`)

  if (!response.ok) {
    throw new Error(`Falha ao consultar ${path}. Status ${response.status}.`)
  }

  return (await response.json()) as T
}

function getStatusTone(status: string): StatusTone {
  if (status === 'healthy') {
    return 'book-state-success'
  }

  if (status === 'degraded') {
    return 'book-state-warning'
  }

  return 'book-state-danger'
}

function buildTopPublishers(livros: Livro[]) {
  const countByPublisher = new Map<string, number>()

  for (const livro of livros) {
    countByPublisher.set(livro.editora, (countByPublisher.get(livro.editora) ?? 0) + 1)
  }

  return [...countByPublisher.entries()]
    .sort((left, right) => right[1] - left[1])
    .slice(0, 5)
}

function buildBooksByYear(livros: Livro[]) {
  const countByYear = new Map<string, number>()

  for (const livro of livros) {
    countByYear.set(livro.anoPublicacao, (countByYear.get(livro.anoPublicacao) ?? 0) + 1)
  }

  return [...countByYear.entries()].sort((left, right) => left[0].localeCompare(right[0]))
}

function formatDate(utcDate: string) {
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(new Date(utcDate))
}

function App() {
  const [snapshot, setSnapshot] = useState<InsightSnapshot | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState('')
  const [searchTerm, setSearchTerm] = useState('')
  const deferredSearchTerm = useDeferredValue(searchTerm)

  useEffect(() => {
    void loadSnapshot()
  }, [])

  async function loadSnapshot() {
    startTransition(() => {
      setIsLoading(true)
      setErrorMessage('')
    })

    try {
      const [health, livros, autores, assuntos] = await Promise.all([
        fetchJson<HealthResponse>('/health'),
        fetchJson<Livro[]>('/livros'),
        fetchJson<Autor[]>('/autores'),
        fetchJson<Assunto[]>('/assuntos'),
      ])

      startTransition(() => {
        setSnapshot({ health, livros, autores, assuntos })
        setIsLoading(false)
      })
    } catch (error) {
      startTransition(() => {
        setErrorMessage(
          error instanceof Error
            ? error.message
            : 'Nao foi possivel carregar os insights do modulo React.',
        )
        setIsLoading(false)
      })
    }
  }

  const livros = snapshot?.livros ?? []
  const autores = snapshot?.autores ?? []
  const assuntos = snapshot?.assuntos ?? []
  const health = snapshot?.health ?? null
  const totalCatalogValue = livros.reduce((sum, livro) => sum + livro.valor, 0)
  const averageBookValue = livros.length ? totalCatalogValue / livros.length : 0
  const latestBooks = [...livros]
    .sort((left, right) => right.anoPublicacao.localeCompare(left.anoPublicacao))
    .slice(0, 5)
  const filteredBooks = latestBooks.filter((livro) => {
    const normalizedSearch = deferredSearchTerm.trim().toLowerCase()

    if (!normalizedSearch) {
      return true
    }

    return [livro.titulo, livro.editora, livro.anoPublicacao]
      .join(' ')
      .toLowerCase()
      .includes(normalizedSearch)
  })
  const topPublishers = buildTopPublishers(livros)
  const booksByYear = buildBooksByYear(livros)
  const highestPublisherCount = topPublishers[0]?.[1] ?? 1
  const highestYearCount = booksByYear.reduce(
    (currentMax, [, value]) => Math.max(currentMax, value),
    1,
  )
  const statusAnnouncement = isLoading
    ? 'Carregando dados do modulo React.'
    : errorMessage
      ? `Falha ao carregar insights: ${errorMessage}`
      : snapshot
        ? 'Painel de insights atualizado com sucesso.'
        : ''
  const apiStatusLabel = health?.status ?? (isLoading ? 'carregando' : 'indisponivel')
  const apiStatusTone = getStatusTone(health?.status ?? 'unhealthy')
  const navigationItems = [
    {
      label: 'Dashboard',
      description: 'Resumo executivo do catalogo',
      href: '#insights-overview',
      iconPath: dashboardIcon,
      current: true,
    },
    {
      label: 'Livros',
      description: 'Catalogo exploravel e valor medio',
      href: '#insights-catalog',
      iconPath: livrosIcon,
      current: false,
    },
    {
      label: 'Autores',
      description: 'Visao de autoria e participacao',
      href: '#insights-authors',
      iconPath: autoresIcon,
      current: false,
    },
    {
      label: 'Assuntos',
      description: 'Taxonomia ativa do projeto',
      href: '#insights-subjects',
      iconPath: assuntosIcon,
      current: false,
    },
  ]

  return (
    <>
      <a className="book-skip-link" href="#book-insights-content">
        Pular para o conteudo principal
      </a>

      <div className="book-sr-only" aria-live="polite">
        {statusAnnouncement}
      </div>

      <div className="book-app-shell">
        <aside className="book-app-sidebar">
          <div className="book-app-brand">
            <span className="book-badge-info">React Insights</span>
            <h1>Book</h1>
            <p>
              Modulo complementar com o mesmo tema azul do Angular, pensado para
              leitura executiva, resumo operacional e demonstracao.
            </p>
          </div>

          <nav className="book-app-nav" aria-label="Navegacao principal do modulo de insights">
            {navigationItems.map((item) => (
              <a
                key={item.label}
                className={`book-app-nav-link ${item.current ? 'book-app-nav-link-current' : ''}`}
                href={item.href}
              >
                <span className="book-app-nav-icon" aria-hidden="true">
                  <img src={item.iconPath} alt="" />
                </span>

                <span className="book-app-nav-copy">
                  <strong>{item.label}</strong>
                  <span>{item.description}</span>
                </span>
              </a>
            ))}
          </nav>

          <div className="book-app-sidebar-footer book-card" aria-live="polite">
            <span className={`book-badge-info ${apiStatusTone}`}>API {apiStatusLabel}</span>
            <p>Backend local em `http://localhost:5268`.</p>
            <button className="book-button-secondary" type="button" onClick={() => void loadSnapshot()}>
              Atualizar indicadores
            </button>
          </div>
        </aside>

        <div className="book-app-main">
          <header className="book-app-topbar">
            <div>
              <p className="book-app-topbar-label">Modulo ativo</p>
              <h2>Book Insights</h2>
            </div>

            <div className="book-app-topbar-actions">
              <span className="book-badge-info">React</span>
              <span className={`book-badge-info ${apiStatusTone}`}>{apiStatusLabel}</span>
              <a className="book-button-secondary insights-link-button" href="http://localhost:4200">
                Abrir painel Angular
              </a>
            </div>
          </header>

          <main id="book-insights-content" className="book-app-content" aria-busy={isLoading}>
            <section id="insights-overview" className="book-page">
              <header className="book-page-header">
                <div>
                  <h1>Painel analitico</h1>
                  <p className="book-page-subtitle">
                    O React agora segue o mesmo shell e o mesmo vocabulário visual
                    do Angular, mantendo foco em leitura, indicadores e apoio de
                    apresentação.
                  </p>
                </div>

                <div className="book-inline-actions">
                  <button className="book-button-primary" type="button" onClick={() => void loadSnapshot()}>
                    Atualizar resumo
                  </button>
                  <a className="book-button-secondary insights-link-button" href="http://localhost:4200">
                    Abrir Angular
                  </a>
                </div>
              </header>

              {errorMessage ? (
                <div className="book-feedback book-feedback-error" role="alert">
                  {errorMessage}
                  <div className="insights-feedback-actions">
                    <button className="book-button-secondary" type="button" onClick={() => void loadSnapshot()}>
                      Tentar novamente
                    </button>
                  </div>
                </div>
              ) : null}

              {isLoading ? (
                <div className="book-card insights-loading" aria-live="polite">
                  <span className="book-loading">Carregando modulo de insights...</span>
                </div>
              ) : null}

              {!isLoading && snapshot ? (
                <>
                  <section className="insights-kpis">
                    <article className="book-card insights-kpi-card">
                      <span className="insights-kpi-label">Livros ativos</span>
                      <strong>{livros.length}</strong>
                      <p>Catalogo principal pronto para o CRUD do painel Angular.</p>
                    </article>

                    <article className="book-card insights-kpi-card">
                      <span className="insights-kpi-label">Autores cadastrados</span>
                      <strong>{autores.length}</strong>
                      <p>Base de autoria disponivel para relatorios e filtros futuros.</p>
                    </article>

                    <article className="book-card insights-kpi-card">
                      <span className="insights-kpi-label">Assuntos mapeados</span>
                      <strong>{assuntos.length}</strong>
                      <p>Taxonomia inicial para classificacao e leitura analitica.</p>
                    </article>

                    <article className="book-card insights-kpi-card">
                      <span className="insights-kpi-label">Valor medio</span>
                      <strong>{currencyFormatter.format(averageBookValue)}</strong>
                      <p>Referencia rapida do patamar financeiro atual do catalogo.</p>
                    </article>
                  </section>

                  <section className="insights-grid">
                    <article className="book-card insights-panel">
                      <div className="insights-panel-heading">
                        <div>
                          <h2 className="book-section-title">Saude da API</h2>
                          <p className="book-page-subtitle">
                            Resumo do endpoint principal e das verificacoes de readiness.
                          </p>
                        </div>
                        <span className={`book-badge-info ${apiStatusTone}`}>{apiStatusLabel}</span>
                      </div>

                      <div className="insights-health-grid">
                        {health?.checks.map((check) => (
                          <div key={check.name} className="insights-health-item">
                            <div>
                              <strong>{check.name}</strong>
                              <p>{check.description}</p>
                            </div>
                            <span className={`book-badge-info ${getStatusTone(check.status)}`}>
                              {check.status}
                            </span>
                          </div>
                        ))}
                      </div>

                      <p className="insights-trace">
                        Atualizado em {formatDate(snapshot.health.utcNow)}. Trace atual: {snapshot.health.traceId}
                      </p>
                    </article>

                    <article className="book-card insights-panel">
                      <div className="insights-panel-heading">
                        <div>
                          <h2 className="book-section-title">Papel do modulo React</h2>
                          <p className="book-page-subtitle">
                            Mesmo tema do Angular, mas com foco complementar em leitura e apresentacao.
                          </p>
                        </div>
                        <img src={dashboardIcon} alt="" className="insights-heading-icon" />
                      </div>

                      <div className="insights-role-grid">
                        <div className="insights-role-card">
                          <strong>book-admin</strong>
                          <p>Pode autenticar no Angular e executar operacoes de escrita no catalogo.</p>
                        </div>
                        <div className="insights-role-card">
                          <strong>book-reader</strong>
                          <p>Pode navegar e validar leitura, recebendo `403` nas rotas protegidas.</p>
                        </div>
                        <div className="insights-role-card">
                          <strong>React insights</strong>
                          <p>Modulo em leitura, orientado a resumo visual, consulta e demonstracao.</p>
                        </div>
                      </div>

                      <div className="insights-inline-note">
                        <span className="book-badge-info">JWT no backend</span>
                        <span className="book-badge-info">CORS local ativo</span>
                        <span className="book-badge-info">Tema compartilhado</span>
                      </div>
                    </article>
                  </section>

                  <section id="insights-catalog" className="insights-grid">
                    <article className="book-card insights-panel">
                      <div className="insights-panel-heading">
                        <div>
                          <h2 className="book-section-title">Editoras em destaque</h2>
                          <p className="book-page-subtitle">
                            Distribuicao simples baseada no acervo atual da API.
                          </p>
                        </div>
                        <img src={livrosIcon} alt="" className="insights-heading-icon" />
                      </div>

                      <div className="insights-bars">
                        {topPublishers.map(([publisher, count]) => (
                          <div key={publisher} className="insights-bar-row">
                            <div className="insights-bar-meta">
                              <strong>{publisher}</strong>
                              <span>{count} livro(s)</span>
                            </div>
                            <div className="insights-bar-track">
                              <div
                                className="insights-bar-fill"
                                style={{ width: `${(count / highestPublisherCount) * 100}%` }}
                              />
                            </div>
                          </div>
                        ))}
                      </div>
                    </article>

                    <article className="book-card insights-panel">
                      <div className="insights-panel-heading">
                        <div>
                          <h2 className="book-section-title">Linha do tempo</h2>
                          <p className="book-page-subtitle">
                            Leitura cronologica simples com base no ano de publicacao.
                          </p>
                        </div>
                        <img src={livrosIcon} alt="" className="insights-heading-icon" />
                      </div>

                      <div className="insights-bars insights-bars-years">
                        {booksByYear.map(([year, count]) => (
                          <div key={year} className="insights-bar-row">
                            <div className="insights-bar-meta">
                              <strong>{year}</strong>
                              <span>{count} item(ns)</span>
                            </div>
                            <div className="insights-bar-track">
                              <div
                                className="insights-bar-fill insights-bar-fill-alt"
                                style={{ width: `${(count / highestYearCount) * 100}%` }}
                              />
                            </div>
                          </div>
                        ))}
                      </div>
                    </article>
                  </section>

                  <section className="insights-grid">
                    <article className="book-card insights-panel insights-panel-wide">
                      <div className="insights-panel-heading">
                        <div>
                          <h2 className="book-section-title">Catalogo exploravel</h2>
                          <p className="book-page-subtitle">
                            Busca local sobre os livros carregados para demonstracao e apoio de leitura.
                          </p>
                        </div>
                        <div className="insights-search-group">
                          <label className="book-sr-only" htmlFor="insights-search">
                            Buscar livros no catalogo
                          </label>
                          <input
                            id="insights-search"
                            className="book-input insights-search"
                            type="search"
                            value={searchTerm}
                            onChange={(event) => setSearchTerm(event.target.value)}
                            placeholder="Buscar por titulo, editora ou ano"
                            aria-describedby="insights-search-hint"
                          />
                          <p id="insights-search-hint" className="book-form-hint">
                            A busca local considera os livros mais recentes carregados neste dashboard.
                          </p>
                        </div>
                      </div>

                      <div className="insights-book-list">
                        {filteredBooks.length ? (
                          filteredBooks.map((livro) => (
                            <article key={livro.codl} className="insights-book-card">
                              <div>
                                <strong>{livro.titulo}</strong>
                                <p>{livro.editora}</p>
                              </div>
                              <div className="insights-book-meta">
                                <span className="book-badge-info">{livro.anoPublicacao}</span>
                                <span className="book-badge-info">{currencyFormatter.format(livro.valor)}</span>
                              </div>
                            </article>
                          ))
                        ) : (
                          <div className="book-empty-state">
                            Nenhum livro encontrado para o termo informado.
                          </div>
                        )}
                      </div>
                    </article>

                    <article id="insights-authors" className="book-card insights-panel">
                      <div className="insights-panel-heading">
                        <h2 className="book-section-title">Autores em destaque</h2>
                        <img src={autoresIcon} alt="" className="insights-heading-icon" />
                      </div>
                      <div className="insights-chip-grid">
                        {autores.map((autor) => (
                          <span key={autor.codAu} className="book-badge-info">
                            {autor.nome}
                          </span>
                        ))}
                      </div>
                    </article>

                    <article id="insights-subjects" className="book-card insights-panel">
                      <div className="insights-panel-heading">
                        <h2 className="book-section-title">Assuntos ativos</h2>
                        <img src={assuntosIcon} alt="" className="insights-heading-icon" />
                      </div>
                      <div className="insights-chip-grid">
                        {assuntos.map((assunto) => (
                          <span key={assunto.codAs} className="book-badge-info">
                            {assunto.descricao}
                          </span>
                        ))}
                      </div>
                    </article>
                  </section>

                  <footer className="insights-footer">
                    <span>Book Insights em React</span>
                    <span>Total estimado do catalogo: {currencyFormatter.format(totalCatalogValue)}</span>
                  </footer>
                </>
              ) : null}
            </section>
          </main>
        </div>
      </div>
    </>
  )
}

export default App
