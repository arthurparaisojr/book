USE BookDb;
GO

ALTER TABLE dbo.Livro
  ADD CONSTRAINT CK_Livro_AnoPublicacao
  CHECK (LEN(AnoPublicacao) = 4 AND AnoPublicacao NOT LIKE '%[^0-9]%');
GO

ALTER TABLE dbo.Livro
  ADD CONSTRAINT CK_Livro_Valor
  CHECK (Valor >= 0);
GO

CREATE INDEX IX_Livro_Titulo ON dbo.Livro (Titulo);
CREATE INDEX IX_Autor_Nome ON dbo.Autor (Nome);
CREATE INDEX IX_Assunto_Descricao ON dbo.Assunto (Descricao);
CREATE INDEX IX_Livro_Autor_Autor ON dbo.Livro_Autor (Autor_CodAu);
CREATE INDEX IX_Livro_Assunto_Assunto ON dbo.Livro_Assunto (Assunto_codAs);
GO
