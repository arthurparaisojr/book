USE BookDb;
GO

CREATE OR ALTER VIEW dbo.vw_RelatorioLivrosPorAutor
AS
SELECT
  a.CodAu,
  a.Nome AS AutorNome,
  l.Codl,
  l.Titulo,
  l.Editora,
  l.Edicao,
  l.AnoPublicacao,
  l.Valor,
  STRING_AGG(s.Descricao, ', ') AS Assuntos
FROM dbo.Livro l
INNER JOIN dbo.Livro_Autor la
  ON la.Livro_Codl = l.Codl
INNER JOIN dbo.Autor a
  ON a.CodAu = la.Autor_CodAu
LEFT JOIN dbo.Livro_Assunto ls
  ON ls.Livro_Codl = l.Codl
LEFT JOIN dbo.Assunto s
  ON s.codAs = ls.Assunto_codAs
GROUP BY
  a.CodAu,
  a.Nome,
  l.Codl,
  l.Titulo,
  l.Editora,
  l.Edicao,
  l.AnoPublicacao,
  l.Valor;
GO
