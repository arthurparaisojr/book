using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using Book.Application.Contracts.Relatorios;
using Book.Application.Exceptions;
using Book.Application.Services.Relatorios;
using Microsoft.Extensions.Configuration;

namespace Book.Infrastructure.Reports;

internal sealed class ChromiumRelatorioPdfService : IRelatorioPdfService
{
    private static readonly string[] BrowserCandidates =
    [
        "/usr/bin/chromium",
        "/usr/bin/chromium-browser",
        "/usr/bin/google-chrome",
        "/usr/bin/google-chrome-stable",
        "chromium",
        "chromium-browser",
        "google-chrome",
        "google-chrome-stable"
    ];

    private readonly string? _configuredBrowserPath;

    public ChromiumRelatorioPdfService(IConfiguration configuration)
    {
        _configuredBrowserPath = configuration["Reports:BrowserPath"]
            ?? Environment.GetEnvironmentVariable("BOOK_REPORT_BROWSER_PATH");
    }

    public async Task<RelatorioPdfResponse> GerarRelatorioLivrosPorAutorAsync(
        GerarRelatorioLivrosPorAutorPdfRequest request,
        CancellationToken cancellationToken = default)
    {
        var browserPath = ResolveBrowserPath();
        var workingDirectory = Path.Combine(
            Path.GetTempPath(),
            "book-relatorios",
            Guid.NewGuid().ToString("N"));

        Directory.CreateDirectory(workingDirectory);

        try
        {
            var htmlPath = Path.Combine(workingDirectory, "relatorio.html");
            var pdfPath = Path.Combine(workingDirectory, "relatorio.pdf");

            await File.WriteAllTextAsync(
                htmlPath,
                BuildHtml(request),
                Encoding.UTF8,
                cancellationToken);

            using var process = CreateChromiumProcess(browserPath, htmlPath, pdfPath);

            if (!process.Start())
            {
                throw new RelatorioPdfGenerationException(
                    "Nao foi possivel iniciar o processo de geracao do PDF do relatorio.");
            }

            var standardErrorTask = process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync(cancellationToken);
            var standardError = await standardErrorTask;

            if (process.ExitCode != 0)
            {
                throw new RelatorioPdfGenerationException(
                    $"A geracao do PDF falhou no navegador headless configurado. Detalhes: {standardError.Trim()}");
            }

            if (!File.Exists(pdfPath))
            {
                throw new RelatorioPdfGenerationException(
                    "O navegador concluiu a execucao, mas o arquivo PDF do relatorio nao foi produzido.");
            }

            var content = await File.ReadAllBytesAsync(pdfPath, cancellationToken);

            if (content.Length == 0)
            {
                throw new RelatorioPdfGenerationException(
                    "O arquivo PDF do relatorio foi gerado vazio.");
            }

            return new RelatorioPdfResponse
            {
                NomeArquivo = BuildFileName(request),
                TipoConteudo = "application/pdf",
                Conteudo = content
            };
        }
        finally
        {
            TryDeleteDirectory(workingDirectory);
        }
    }

    private string ResolveBrowserPath()
    {
        if (!string.IsNullOrWhiteSpace(_configuredBrowserPath))
        {
            return _configuredBrowserPath;
        }

        foreach (var candidate in BrowserCandidates)
        {
            if (IsAbsoluteExecutable(candidate))
            {
                return candidate;
            }

            if (TryResolveFromPath(candidate, out var resolvedPath))
            {
                return resolvedPath!;
            }
        }

        throw new RelatorioPdfGenerationException(
            "Nenhum executavel Chromium/Chrome foi encontrado para gerar o PDF do relatorio. " +
            "Use a stack Docker completa ou configure Reports:BrowserPath.");
    }

    private static bool IsAbsoluteExecutable(string path)
    {
        return Path.IsPathRooted(path) && File.Exists(path);
    }

    private static bool TryResolveFromPath(string command, out string? resolvedPath)
    {
        resolvedPath = null;

        var pathValue = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrWhiteSpace(pathValue))
        {
            return false;
        }

        var extensions = OperatingSystem.IsWindows()
            ? (Environment.GetEnvironmentVariable("PATHEXT")?.Split(';', StringSplitOptions.RemoveEmptyEntries)
                ?? [".exe", ".cmd", ".bat"])
            : [string.Empty];

        foreach (var directory in pathValue.Split(Path.PathSeparator, StringSplitOptions.RemoveEmptyEntries))
        {
            foreach (var extension in extensions)
            {
                var candidate = Path.Combine(directory, command + extension);
                if (File.Exists(candidate))
                {
                    resolvedPath = candidate;
                    return true;
                }
            }
        }

        return false;
    }

    private static Process CreateChromiumProcess(string browserPath, string htmlPath, string pdfPath)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = browserPath,
            RedirectStandardError = true,
            RedirectStandardOutput = false,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("--headless=new");
        startInfo.ArgumentList.Add("--disable-gpu");
        startInfo.ArgumentList.Add("--disable-dev-shm-usage");
        startInfo.ArgumentList.Add("--no-sandbox");
        startInfo.ArgumentList.Add("--no-first-run");
        startInfo.ArgumentList.Add("--disable-extensions");
        startInfo.ArgumentList.Add("--allow-file-access-from-files");
        startInfo.ArgumentList.Add("--print-to-pdf-no-header");
        startInfo.ArgumentList.Add("--virtual-time-budget=2000");
        startInfo.ArgumentList.Add($"--print-to-pdf={pdfPath}");
        startInfo.ArgumentList.Add(new Uri(htmlPath).AbsoluteUri);

        return new Process
        {
            StartInfo = startInfo
        };
    }

    private static string BuildHtml(GerarRelatorioLivrosPorAutorPdfRequest request)
    {
        var culture = CultureInfo.GetCultureInfo("pt-BR");
        var groupedItems = request.Itens
            .GroupBy(item => item.AutorNome)
            .OrderBy(group => group.Key, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();
        var totalAutores = groupedItems.Length;
        var totalLivros = request.Itens.Count;
        var valorTotal = request.Itens.Sum(item => item.Valor);
        var filtroAutor = string.IsNullOrWhiteSpace(request.AutorNomeFiltro)
            ? "Todos os autores"
            : request.AutorNomeFiltro.Trim();
        var generatedAt = request.GeradoEm.ToString("dd/MM/yyyy HH:mm", culture);

        var builder = new StringBuilder();
        builder.Append(
            """
            <!DOCTYPE html>
            <html lang="pt-BR">
            <head>
              <meta charset="utf-8">
              <meta name="viewport" content="width=device-width, initial-scale=1">
              <title>Relatorio de Livros por Autor</title>
              <link
                href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
                rel="stylesheet"
                integrity="sha384-QWTKZyjpPEjISv5WaRU9OFeRpok6YctnYmDr5pNlyT2bRjXh0JMhjY6hW+ALEwIH"
                crossorigin="anonymous">
              <style>
                :root {
                  --book-primary: #1d4ed8;
                  --book-primary-soft: #dbeafe;
                  --book-accent: #0ea5e9;
                  --book-surface: #ffffff;
                  --book-surface-soft: #eff6ff;
                  --book-border: #bfdbfe;
                  --book-text: #16324f;
                  --book-text-muted: #5b7491;
                }

                body {
                  background: linear-gradient(180deg, #f8fbff 0%, #eef5ff 100%);
                  color: var(--book-text);
                  font-family: "Segoe UI", Tahoma, sans-serif;
                }

                .report-shell {
                  padding: 32px 0 40px;
                }

                .report-hero {
                  background: linear-gradient(135deg, var(--book-primary) 0%, var(--book-accent) 100%);
                  border-radius: 24px;
                  color: #ffffff;
                  padding: 28px;
                  box-shadow: 0 18px 48px rgba(29, 78, 216, 0.18);
                }

                .report-card {
                  background: var(--book-surface);
                  border: 1px solid var(--book-border);
                  border-radius: 20px;
                  box-shadow: 0 10px 30px rgba(37, 99, 235, 0.08);
                }

                .report-kpi {
                  background: var(--book-surface-soft);
                  border: 1px solid var(--book-border);
                  border-radius: 18px;
                  min-height: 100%;
                }

                .report-kpi .label {
                  color: var(--book-text-muted);
                  font-size: 0.88rem;
                  text-transform: uppercase;
                  letter-spacing: 0.04em;
                }

                .report-kpi .value {
                  color: var(--book-primary);
                  font-size: 1.8rem;
                  font-weight: 700;
                  line-height: 1.1;
                }

                .report-section-title {
                  color: var(--book-primary);
                }

                .report-author-card {
                  border: 1px solid var(--book-border);
                  border-radius: 18px;
                  overflow: hidden;
                }

                .report-author-header {
                  background: linear-gradient(135deg, #eff6ff 0%, #dbeafe 100%);
                }

                .table {
                  margin-bottom: 0;
                }

                .table thead th {
                  background: #eff6ff;
                  color: var(--book-primary);
                  font-size: 0.82rem;
                  text-transform: uppercase;
                  letter-spacing: 0.04em;
                }

                .table td,
                .table th {
                  border-color: #dbeafe;
                  vertical-align: top;
                }

                .report-badge {
                  background: #e0f2fe;
                  color: #075985;
                  border: 1px solid #bae6fd;
                  border-radius: 999px;
                  display: inline-block;
                  font-size: 0.78rem;
                  margin: 0 8px 8px 0;
                  padding: 0.35rem 0.7rem;
                }

                .report-footer-note {
                  color: var(--book-text-muted);
                  font-size: 0.9rem;
                }

                @media print {
                  body {
                    background: #ffffff;
                  }

                  .report-shell {
                    padding: 0;
                  }
                }
              </style>
            </head>
            <body>
              <main class="container report-shell">
            """);

        builder.AppendLine(
            $"""
                <section class="report-hero mb-4">
                  <div class="row g-4 align-items-end">
                    <div class="col-lg-8">
                      <span class="badge text-bg-light text-primary fw-semibold mb-3">TJ-JUD</span>
                      <h1 class="display-6 fw-bold mb-2">Relatorio de Livros por Autor</h1>
                      <p class="mb-0 fs-5">
                        Documento obrigatorio gerado a partir da view oficial do banco com
                        dados de livros, autores e assuntos.
                      </p>
                    </div>
                    <div class="col-lg-4">
                      <div class="bg-white bg-opacity-10 rounded-4 p-3">
                        <div class="small text-uppercase fw-semibold opacity-75">Filtro aplicado</div>
                        <div class="fs-5 fw-semibold">{HtmlEncode(filtroAutor)}</div>
                        <div class="small mt-3 opacity-75">Gerado em {HtmlEncode(generatedAt)}</div>
                      </div>
                    </div>
                  </div>
                </section>
            """);

        builder.AppendLine(
            $"""
                <section class="report-card p-4 mb-4">
                  <div class="row g-3">
                    <div class="col-md-4">
                      <div class="report-kpi p-3">
                        <div class="label mb-2">Autores no relatorio</div>
                        <div class="value">{totalAutores}</div>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="report-kpi p-3">
                        <div class="label mb-2">Livros listados</div>
                        <div class="value">{totalLivros}</div>
                      </div>
                    </div>
                    <div class="col-md-4">
                      <div class="report-kpi p-3">
                        <div class="label mb-2">Valor total</div>
                        <div class="value">{HtmlEncode(valorTotal.ToString("C", culture))}</div>
                      </div>
                    </div>
                  </div>
                </section>
            """);

        builder.AppendLine(
            """
                <section class="mb-4">
                  <div class="d-flex justify-content-between align-items-end mb-3">
                    <div>
                      <h2 class="h4 fw-bold report-section-title mb-1">Detalhamento agrupado por autor</h2>
                      <p class="text-secondary mb-0">Cada bloco abaixo representa a leitura consolidada da view oficial do relatorio.</p>
                    </div>
                  </div>
            """);

        if (groupedItems.Length == 0)
        {
            builder.AppendLine(
                """
                  <div class="report-card p-4">
                    <div class="alert alert-info mb-0" role="alert">
                      Nenhum item foi encontrado para o filtro informado.
                    </div>
                  </div>
                """);
        }
        else
        {
            foreach (var group in groupedItems)
            {
                var quantidadeLivros = group.Count();
                var valorAutor = group.Sum(item => item.Valor);

                builder.AppendLine(
                    $"""
                      <article class="report-author-card report-card mb-4">
                        <div class="report-author-header p-4 border-bottom">
                          <div class="d-flex flex-wrap justify-content-between gap-3 align-items-end">
                            <div>
                              <h3 class="h4 fw-bold mb-1">{HtmlEncode(group.Key)}</h3>
                              <p class="text-secondary mb-0">{quantidadeLivros} livro(s) listado(s) para este autor.</p>
                            </div>
                            <div class="text-end">
                              <div class="small text-uppercase fw-semibold text-secondary">Valor consolidado</div>
                              <div class="fs-5 fw-bold text-primary">{HtmlEncode(valorAutor.ToString("C", culture))}</div>
                            </div>
                          </div>
                        </div>
                        <div class="table-responsive">
                          <table class="table table-striped align-middle">
                            <thead>
                              <tr>
                                <th scope="col">Titulo</th>
                                <th scope="col">Editora</th>
                                <th scope="col" class="text-center">Edicao</th>
                                <th scope="col" class="text-center">Ano</th>
                                <th scope="col" class="text-end">Valor</th>
                                <th scope="col">Assuntos</th>
                              </tr>
                            </thead>
                            <tbody>
                    """);

                foreach (var item in group.OrderBy(row => row.Titulo, StringComparer.CurrentCultureIgnoreCase))
                {
                    builder.AppendLine(
                        $"""
                              <tr>
                                <td class="fw-semibold">{HtmlEncode(item.Titulo)}</td>
                                <td>{HtmlEncode(item.Editora)}</td>
                                <td class="text-center">{item.Edicao}</td>
                                <td class="text-center">{HtmlEncode(item.AnoPublicacao)}</td>
                                <td class="text-end">{HtmlEncode(item.Valor.ToString("C", culture))}</td>
                                <td>{BuildAssuntosBadges(item.Assuntos)}</td>
                              </tr>
                        """);
                }

                builder.AppendLine(
                    """
                            </tbody>
                          </table>
                        </div>
                      </article>
                    """);
            }
        }

        builder.AppendLine(
            """
                </section>
            """);

        builder.AppendLine(
            """
                <footer class="report-footer-note pt-2">
                  Fonte oficial: dbo.vw_RelatorioLivrosPorAutor.
                </footer>
              </main>
            </body>
            </html>
            """);

        return builder.ToString();
    }

    private static string BuildAssuntosBadges(string assuntos)
    {
        if (string.IsNullOrWhiteSpace(assuntos))
        {
            return "<span class=\"text-secondary\">Sem assunto vinculado</span>";
        }

        var tags = assuntos
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(assunto => $"<span class=\"report-badge\">{HtmlEncode(assunto)}</span>");

        return string.Concat(tags);
    }

    private static string BuildFileName(GerarRelatorioLivrosPorAutorPdfRequest request)
    {
        var slugFiltro = string.IsNullOrWhiteSpace(request.AutorNomeFiltro)
            ? "todos-os-autores"
            : Slugify(request.AutorNomeFiltro);

        return $"relatorio-livros-por-autor-{slugFiltro}-{request.GeradoEm:yyyyMMdd-HHmmss}.pdf";
    }

    private static string Slugify(string value)
    {
        var normalized = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var character in normalized)
        {
            var category = CharUnicodeInfo.GetUnicodeCategory(character);

            if (category == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                builder.Append(char.ToLowerInvariant(character));
                continue;
            }

            if (builder.Length > 0 && builder[^1] != '-')
            {
                builder.Append('-');
            }
        }

        return builder.ToString().Trim('-');
    }

    private static string HtmlEncode(string value)
    {
        return WebUtility.HtmlEncode(value);
    }

    private static void TryDeleteDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            return;
        }

        try
        {
            Directory.Delete(path, true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
