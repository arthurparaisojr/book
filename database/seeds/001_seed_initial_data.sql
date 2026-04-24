USE BookDb;
GO

INSERT INTO dbo.Autor (Nome)
VALUES
  ('Martin Fowler'),
  ('Robert C. Martin'),
  ('Eric Evans');

INSERT INTO dbo.Assunto (Descricao)
VALUES
  ('Arquitetura'),
  ('DDD'),
  ('Clean Code');

INSERT INTO dbo.Livro (Titulo, Editora, Edicao, AnoPublicacao, Valor)
VALUES
  ('Patterns of Enterprise Application Architecture', 'Addison-Wesley', 1, '2002', 199.90),
  ('Clean Code', 'Prentice Hall', 1, '2008', 149.90);

INSERT INTO dbo.Livro_Autor (Livro_Codl, Autor_CodAu)
VALUES
  (1, 1),
  (2, 2);

INSERT INTO dbo.Livro_Assunto (Livro_Codl, Assunto_codAs)
VALUES
  (1, 1),
  (2, 3);
GO
