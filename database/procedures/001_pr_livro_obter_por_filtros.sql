USE BookDb;
GO

CREATE OR ALTER PROCEDURE dbo.pr_Livro_ObterPorFiltros
  @Titulo VARCHAR(40) = NULL,
  @AutorNome VARCHAR(40) = NULL,
  @AssuntoDescricao VARCHAR(20) = NULL
AS
BEGIN
  SET NOCOUNT ON;

  SELECT DISTINCT
    l.Codl,
    l.Titulo,
    l.Editora,
    l.Edicao,
    l.AnoPublicacao,
    l.Valor
  FROM dbo.Livro l
  LEFT JOIN dbo.Livro_Autor la
    ON la.Livro_Codl = l.Codl
  LEFT JOIN dbo.Autor a
    ON a.CodAu = la.Autor_CodAu
  LEFT JOIN dbo.Livro_Assunto ls
    ON ls.Livro_Codl = l.Codl
  LEFT JOIN dbo.Assunto s
    ON s.codAs = ls.Assunto_codAs
  WHERE (@Titulo IS NULL OR l.Titulo LIKE '%' + @Titulo + '%')
    AND (@AutorNome IS NULL OR a.Nome LIKE '%' + @AutorNome + '%')
    AND (@AssuntoDescricao IS NULL OR s.Descricao LIKE '%' + @AssuntoDescricao + '%')
  ORDER BY l.Titulo;
END;
GO
