import { useDeferredValue, useEffect, useState, startTransition } from 'react'
import dashboardIcon from '../../../shared/icons/svg/book-nav-dashboard.svg'
import livrosIcon from '../../../shared/icons/svg/book-nav-livros.svg'
import autoresIcon from '../../../shared/icons/svg/book-nav-autores.svg'
import assuntosIcon from '../../../shared/icons/svg/book-nav-assuntos.svg'
import './App.css'

const API_BASE_URL =
  import.meta.env.VITE_API_BASE_URL ?? '/api/v1'
const AUTH_STORAGE_KEY = 'book-insights-session'

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

interface RelatorioLivroPorAutor {
  codAu: number
  autorNome: string
  codl: number
  titulo: string
  editora: string
  edicao: number
  anoPublicacao: string
  valor: number
  assuntos: string
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
  relatorioLivrosPorAutor: RelatorioLivroPorAutor[]
}

interface LoginRequest {
  username: string
  password: string
}

interface AuthTokenResponse {
  accessToken: string
  tokenType: string
  expiresAtUtc: string
  username: string
  role: string
}

interface AuthSession {
  accessToken: string
  expiresAtUtc: string
  username: string
  role: string
}

type StatusTone = 'book-state-success' | 'book-state-warning' | 'book-state-danger'

const currencyFormatter = new Intl.NumberFormat('pt-BR', {
  style: 'currency',
  currency: 'BRL',
})

function readStoredSession(): AuthSession | null {
  const rawSession = localStorage.getItem(AUTH_STORAGE_KEY)

  if (!rawSession) {
    return null
  }

  try {
    return JSON.parse(rawSession) as AuthSession
  } catch {
    localStorage.removeItem(AUTH_STORAGE_KEY)
    return null
  }
}

function persistSession(session: AuthSession | null) {
  if (session) {
    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(session))
    return
  }

  localStorage.removeItem(AUTH_STORAGE_KEY)
}

async function buildApiErrorMessage(response: Response) {
  try {
    const body = (await response.json()) as {
      detail?: string
      title?: string
      errors?: Record<string, string[]>
    }

    if (body.detail) {
      return body.detail
    }

    const firstError = body.errors ? Object.values(body.errors).flat()[0] : ''
    if (firstError) {
      return firstError
    }

    if (body.title) {
      return body.title
    }
  } catch {
    // Ignora erros de parse e cai para a mensagem padrao abaixo.
  }

  return `Falha ao consultar a API. Status ${response.status}.`
}

async function fetchJson<T>(path: string, accessToken?: string | null): Promise<T> {
  const response = await fetch(`${API_BASE_URL}${path}`, {
    headers: accessToken
      ? {
          Authorization: `Bearer ${accessToken}`,
        }
      : undefined,
  })

  if (!response.ok) {
    throw new Error(await buildApiErrorMessage(response))
  }

  return (await response.json()) as T
}

async function authenticate(request: LoginRequest): Promise<AuthSession> {
  const response = await fetch(`${API_BASE_URL}/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      Accept: 'application/json',
    },
    body: JSON.stringify(request),
  })

  if (!response.ok) {
    throw new Error(await buildApiErrorMessage(response))
  }

  const payload = (await response.json()) as AuthTokenResponse

  return {
    accessToken: payload.accessToken,
    expiresAtUtc: payload.expiresAtUtc,
    username: payload.username,
    role: payload.role,
  }
}

function buildReportPdfPath(authorFilter: string) {
  const normalizedAuthorFilter = authorFilter.trim()

  if (!normalizedAuthorFilter) {
    return '/relatorios/livros-por-autor/pdf'
  }

  return `/relatorios/livros-por-autor/pdf?autorNome=${encodeURIComponent(normalizedAuthorFilter)}`
}

function resolveDownloadFileName(contentDisposition: string | null, fallbackName: string) {
  if (!contentDisposition) {
    return fallbackName
  }

  const utf8Match = contentDisposition.match(/filename\*=UTF-8''([^;]+)/i)
  if (utf8Match?.[1]) {
    return decodeURIComponent(utf8Match[1])
  }

  const asciiMatch = contentDisposition.match(/filename="?([^";]+)"?/i)
  if (asciiMatch?.[1]) {
    return asciiMatch[1]
  }

  return fallbackName
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

function buildReportSummaryByAuthor(rows: RelatorioLivroPorAutor[]) {
  const summaryByAuthor = new Map<string, { count: number; totalValue: number }>()

  for (const row of rows) {
    const current = summaryByAuthor.get(row.autorNome) ?? { count: 0, totalValue: 0 }
    summaryByAuthor.set(row.autorNome, {
      count: current.count + 1,
      totalValue: current.totalValue + row.valor,
    })
  }

  return [...summaryByAuthor.entries()]
    .map(([autorNome, summary]) => ({
      autorNome,
      count: summary.count,
      totalValue: summary.totalValue,
    }))
    .sort((left, right) => right.count - left.count || left.autorNome.localeCompare(right.autorNome))
    .slice(0, 4)
}

function formatDate(utcDate: string) {
  return new Intl.DateTimeFormat('pt-BR', {
    dateStyle: 'short',
    timeStyle: 'short',
  }).format(new Date(utcDate))
}

function App() {
  const [session, setSession] = useState<AuthSession | null>(() => readStoredSession())
  const [loginForm, setLoginForm] = useState<LoginRequest>({ username: '', password: '' })
  const [loginErrorMessage, setLoginErrorMessage] = useState('')
  const [isAuthenticating, setIsAuthenticating] = useState(false)
  const [snapshot, setSnapshot] = useState<InsightSnapshot | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [errorMessage, setErrorMessage] = useState('')
  const [searchTerm, setSearchTerm] = useState('')
  const [reportSearchTerm, setReportSearchTerm] = useState('')
  const [isDownloadingReport, setIsDownloadingReport] = useState(false)
  const [reportDownloadMessage, setReportDownloadMessage] = useState('')
  const deferredSearchTerm = useDeferredValue(searchTerm)
  const deferredReportSearchTerm = useDeferredValue(reportSearchTerm)

  useEffect(() => {
    if (!session) {
      setSnapshot(null)
      setIsLoading(false)
      return
    }

    void loadSnapshot(session.accessToken)
  }, [session])

  async function loadSnapshot(accessToken = session?.accessToken ?? null) {
    startTransition(() => {
      setIsLoading(true)
      setErrorMessage('')
    })

    try {
      const [health, livros, autores, assuntos, relatorioLivrosPorAutor] = await Promise.all([
        fetchJson<HealthResponse>('/health', accessToken),
        fetchJson<Livro[]>('/livros', accessToken),
        fetchJson<Autor[]>('/autores', accessToken),
        fetchJson<Assunto[]>('/assuntos', accessToken),
        fetchJson<RelatorioLivroPorAutor[]>('/relatorios/livros-por-autor', accessToken),
      ])

      startTransition(() => {
        setSnapshot({ health, livros, autores, assuntos, relatorioLivrosPorAutor })
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

  function updateLoginForm<K extends keyof LoginRequest>(field: K, value: LoginRequest[K]) {
    setLoginForm((current) => ({
      ...current,
      [field]: value,
    }))
  }

  function useCredentialPreset(username: string, password: string) {
    setLoginForm({ username, password })
    setLoginErrorMessage('')
  }

  async function submitLogin(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()

    if (!loginForm.username.trim() || !loginForm.password.trim()) {
      setLoginErrorMessage('Informe usuario e senha para autenticar no modulo React.')
      return
    }

    setIsAuthenticating(true)
    setLoginErrorMessage('')

    try {
      const authenticatedSession = await authenticate({
        username: loginForm.username.trim(),
        password: loginForm.password,
      })

      persistSession(authenticatedSession)
      setSession(authenticatedSession)
    } catch (error) {
      setLoginErrorMessage(
        error instanceof Error ? error.message : 'Nao foi possivel autenticar no modulo React.',
      )
    } finally {
      setIsAuthenticating(false)
    }
  }

  function logout() {
    persistSession(null)
    setSession(null)
    setSnapshot(null)
    setErrorMessage('')
    setLoginErrorMessage('')
  }

  async function downloadReportPdf() {
    setIsDownloadingReport(true)
    setReportDownloadMessage('')

    try {
      const response = await fetch(`${API_BASE_URL}${buildReportPdfPath(reportSearchTerm)}`, {
        headers: {
          Accept: 'application/pdf',
        },
      })

      if (!response.ok) {
        throw new Error(await buildApiErrorMessage(response))
      }

      const fileName = resolveDownloadFileName(
        response.headers.get('Content-Disposition'),
        'relatorio-livros-por-autor.pdf',
      )
      const blob = await response.blob()
      const objectUrl = window.URL.createObjectURL(blob)
      const temporaryLink = document.createElement('a')

      temporaryLink.href = objectUrl
      temporaryLink.download = fileName
      document.body.append(temporaryLink)
      temporaryLink.click()
      temporaryLink.remove()
      window.URL.revokeObjectURL(objectUrl)

      setReportDownloadMessage(
        reportSearchTerm.trim()
          ? 'PDF gerado com o filtro de autor informado.'
          : 'PDF completo do relatorio gerado com sucesso.',
      )
    } catch (error) {
      setReportDownloadMessage(
        error instanceof Error
          ? `Falha ao gerar o PDF: ${error.message}`
          : 'Falha ao gerar o PDF do relatorio.',
      )
    } finally {
      setIsDownloadingReport(false)
    }
  }

  const livros = snapshot?.livros ?? []
  const autores = snapshot?.autores ?? []
  const assuntos = snapshot?.assuntos ?? []
  const reportRows = snapshot?.relatorioLivrosPorAutor ?? []
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
  const filteredReportRows = reportRows.filter((row) => {
    const normalizedSearch = deferredReportSearchTerm.trim().toLowerCase()

    if (!normalizedSearch) {
      return true
    }

    return [row.autorNome, row.titulo, row.editora, row.assuntos]
      .join(' ')
      .toLowerCase()
      .includes(normalizedSearch)
  })
  const reportSummaryByAuthor = buildReportSummaryByAuthor(filteredReportRows)
  const highestPublisherCount = topPublishers[0]?.[1] ?? 1
  const highestYearCount = booksByYear.reduce(
    (currentMax, [, value]) => Math.max(currentMax, value),
    1,
  )
  const statusAnnouncement = isLoading
    ? 'Carregando dados do modulo React.'
    : !session
      ? 'Modulo React aguardando autenticacao.'
    : errorMessage
      ? `Falha ao carregar insights: ${errorMessage}`
      : snapshot
        ? 'Painel de insights atualizado com sucesso.'
        : ''
  const apiStatusLabel = health?.status ?? (isLoading ? 'carregando' : 'indisponivel')
  const apiStatusTone = getStatusTone(health?.status ?? 'unhealthy')
  const navigationItems = [
    {
      label: 'Visao Geral',
      description: 'Resumo executivo do catalogo',
      href: '#insights-overview',
      iconPath: dashboardIcon,
      current: true,
    },
    {
      label: 'Insights de Livros',
      description: 'Catalogo exploravel, valor medio e tendencias',
      href: '#insights-catalog',
      iconPath: livrosIcon,
      current: false,
    },
    {
      label: 'Relatorio por Autor',
      description: 'Leitura detalhada da view do banco',
      href: '#insights-report',
      iconPath: livrosIcon,
      current: false,
    },
    {
      label: 'Insights de Autores',
      description: 'Visao de autoria e participacao',
      href: '#insights-authors',
      iconPath: autoresIcon,
      current: false,
    },
    {
      label: 'Insights de Assuntos',
      description: 'Taxonomia ativa do projeto',
      href: '#insights-subjects',
      iconPath: assuntosIcon,
      current: false,
    },
  ]

  if (!session) {
    return (
      <>
        <a className="book-skip-link" href="#book-insights-login">
          Pular para o conteudo principal
        </a>

        <div className="book-sr-only" aria-live="polite">
          {statusAnnouncement}
        </div>

        <main id="book-insights-login" className="insights-login-shell">
          <section className="insights-login-panel book-card">
            <span className="book-badge-info">React Insights</span>
            <h1>Acesso analitico do projeto Book</h1>
            <p>
              Este modulo React agora tambem autentica com usuario e senha, usando o
              mesmo endpoint de login do Angular e a mesma API `.NET 8`.
            </p>

            <div className="insights-login-grid">
              <article className="book-card insights-role-card">
                <strong>Fluxo recomendado</strong>
                <p>Autentique para carregar os indicadores, o relatorio por autor e o painel de saude.</p>
              </article>
              <article className="book-card insights-role-card">
                <strong>Mesmas credenciais do Angular</strong>
                <p>`book-admin` e `book-reader` funcionam aqui tambem, com o mesmo backend JWT.</p>
              </article>
            </div>

            <div className="insights-inline-note">
              <button
                className="book-button-secondary"
                type="button"
                onClick={() => useCredentialPreset('book-admin', 'Book@123')}
              >
                Usar book-admin
              </button>
              <button
                className="book-button-secondary"
                type="button"
                onClick={() => useCredentialPreset('book-reader', 'Book@123')}
              >
                Usar book-reader
              </button>
              <a className="book-button-secondary insights-link-button" href="http://localhost:4200">
                Abrir Angular
              </a>
            </div>
          </section>

          <form className="insights-login-panel book-card" onSubmit={(event) => void submitLogin(event)} noValidate>
            <div>
              <h2 className="book-section-title">Entrar</h2>
              <p className="book-page-subtitle">
                Use usuario e senha para acessar o modulo React com as mesmas credenciais do Angular.
              </p>
            </div>

            <div className="book-form-grid">
              <div className="book-form-field book-form-field-full">
                <label htmlFor="react-username">Usuario</label>
                <input
                  id="react-username"
                  className="book-input"
                  type="text"
                  autoComplete="username"
                  value={loginForm.username}
                  onChange={(event) => updateLoginForm('username', event.target.value)}
                />
              </div>

              <div className="book-form-field book-form-field-full">
                <label htmlFor="react-password">Senha</label>
                <input
                  id="react-password"
                  className="book-input"
                  type="password"
                  autoComplete="current-password"
                  value={loginForm.password}
                  onChange={(event) => updateLoginForm('password', event.target.value)}
                />
              </div>
            </div>

            {loginErrorMessage ? (
              <div className="book-feedback book-feedback-error" role="alert">
                {loginErrorMessage}
              </div>
            ) : null}

            <div className="insights-inline-note">
              <span className="book-badge-info">JWT Bearer</span>
              <span className="book-badge-info">Mesmo backend do Angular</span>
              <span className="book-badge-info">Leitura analitica protegida</span>
            </div>

            <button className="book-button-primary" type="submit" disabled={isAuthenticating}>
              {isAuthenticating ? 'Autenticando...' : 'Entrar no modulo'}
            </button>
          </form>
        </main>
      </>
    )
  }

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
            <p>
              Sessao de <strong>{session.username}</strong> com perfil <strong>{session.role}</strong>.
            </p>
            <button className="book-button-secondary" type="button" onClick={() => void loadSnapshot()}>
              Atualizar indicadores
            </button>
          </div>
        </aside>

        <div className="book-app-main">
          <header className="book-app-topbar">
            <div>
              <p className="book-app-topbar-label">Sessao ativa</p>
              <h2>{session.username}</h2>
            </div>

            <div className="book-app-topbar-actions">
              <span className="book-badge-info">{session.role}</span>
              <span className={`book-badge-info ${apiStatusTone}`}>{apiStatusLabel}</span>
              <a className="book-button-secondary insights-link-button" href="http://localhost:4200">
                Abrir painel Angular
              </a>
              <button className="book-button-secondary" type="button" onClick={logout}>
                Sair
              </button>
            </div>
          </header>

          <main id="book-insights-content" className="book-app-content" aria-busy={isLoading}>
            <section id="insights-overview" className="book-page">
              <header className="book-page-header">
                <div>
                  <h1>Painel analitico</h1>
                  <p className="book-page-subtitle">
                    O React agora segue o mesmo shell e o mesmo vocabulario visual
                    do Angular, mantendo foco em leitura, indicadores e apoio de
                    apresentacao.
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

                  <section id="insights-report" className="book-page">
                    <header className="book-page-header">
                      <div>
                        <h2 className="book-section-title">Relatorio de livros por autor</h2>
                        <p className="book-page-subtitle">
                          Este bloco consome a `view` `vw_RelatorioLivrosPorAutor` pela API e
                          mostra a leitura detalhada do acervo por autoria.
                        </p>
                      </div>

                      <div className="insights-search-group">
                        <div className="book-inline-actions insights-report-actions">
                          <button
                            className="book-button-secondary"
                            type="button"
                            onClick={() => void downloadReportPdf()}
                            disabled={isDownloadingReport}
                          >
                            {isDownloadingReport ? 'Gerando PDF...' : 'Baixar PDF do relatorio'}
                          </button>
                        </div>
                        <label className="book-sr-only" htmlFor="insights-report-search">
                          Filtrar relatorio por autor, titulo, editora ou assunto
                        </label>
                        <input
                          id="insights-report-search"
                          className="book-input insights-search"
                          type="search"
                          value={reportSearchTerm}
                          onChange={(event) => setReportSearchTerm(event.target.value)}
                          placeholder="Filtrar relatorio por autor, titulo ou assunto"
                          aria-describedby="insights-report-search-hint"
                        />
                        <p id="insights-report-search-hint" className="book-form-hint">
                          O filtro facilita a leitura do relatorio detalhado retornado pela API.
                        </p>
                        <p className="book-form-hint">
                          O download do PDF usa o termo atual como filtro de autor na API. Sem
                          busca preenchida, o arquivo sai completo.
                        </p>
                        {reportDownloadMessage ? (
                          <div
                            className={`book-feedback ${
                              reportDownloadMessage.startsWith('Falha')
                                ? 'book-feedback-error'
                                : 'book-feedback-success'
                            }`}
                            role="status"
                          >
                            {reportDownloadMessage}
                          </div>
                        ) : null}
                      </div>
                    </header>

                    <section className="insights-kpis insights-kpis-compact">
                      <article className="book-card insights-kpi-card">
                        <span className="insights-kpi-label">Linhas do relatorio</span>
                        <strong>{filteredReportRows.length}</strong>
                        <p>Quantidade de combinacoes livro x autor retornadas pela view.</p>
                      </article>

                      <article className="book-card insights-kpi-card">
                        <span className="insights-kpi-label">Autores no relatorio</span>
                        <strong>{new Set(filteredReportRows.map((row) => row.autorNome)).size}</strong>
                        <p>Autores representados na leitura atual do relatorio.</p>
                      </article>
                    </section>

                    <section className="insights-report-summary">
                      {reportSummaryByAuthor.map((item) => (
                        <article key={item.autorNome} className="book-card insights-role-card">
                          <strong>{item.autorNome}</strong>
                          <p>{item.count} livro(s) no relatorio atual.</p>
                          <span className="book-badge-info">
                            {currencyFormatter.format(item.totalValue)}
                          </span>
                        </article>
                      ))}
                    </section>

                    <div className="table-wrapper">
                      <table className="book-table">
                        <thead>
                          <tr>
                            <th>Autor</th>
                            <th>Titulo</th>
                            <th>Editora</th>
                            <th>Ano</th>
                            <th>Valor</th>
                            <th>Assuntos</th>
                          </tr>
                        </thead>
                        <tbody>
                          {filteredReportRows.length ? (
                            filteredReportRows.map((row) => (
                              <tr key={`${row.codAu}-${row.codl}`}>
                                <td>{row.autorNome}</td>
                                <td>{row.titulo}</td>
                                <td>{row.editora}</td>
                                <td>{row.anoPublicacao}</td>
                                <td>{currencyFormatter.format(row.valor)}</td>
                                <td>{row.assuntos || 'Sem assuntos vinculados'}</td>
                              </tr>
                            ))
                          ) : (
                            <tr>
                              <td colSpan={6} className="insights-report-empty">
                                Nenhum item encontrado para o filtro atual do relatorio.
                              </td>
                            </tr>
                          )}
                        </tbody>
                      </table>
                    </div>
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
