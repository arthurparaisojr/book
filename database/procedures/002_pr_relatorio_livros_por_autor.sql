USE BookDb;
GO

CREATE OR ALTER PROCEDURE dbo.pr_RelatorioLivrosPorAutor
  @AutorNome VARCHAR(40) = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT
    CodAu,
    AutorNome,
    Codl,
    Titulo,
    Editora,
    Edicao,
    AnoPublicacao,
    Valor,
    Assuntos
  FROM dbo.vw_RelatorioLivrosPorAutor
  WHERE (@AutorNome IS NULL OR AutorNome LIKE '%' + @AutorNome + '%')
  ORDER BY AutorNome, Titulo;
END;
GO
