IF DB_ID('BookDb') IS NULL
BEGIN
  CREATE DATABASE BookDb;
END
GO

USE BookDb;
GO

IF OBJECT_ID('dbo.Livro_Autor', 'U') IS NOT NULL DROP TABLE dbo.Livro_Autor;
IF OBJECT_ID('dbo.Livro_Assunto', 'U') IS NOT NULL DROP TABLE dbo.Livro_Assunto;
IF OBJECT_ID('dbo.Livro', 'U') IS NOT NULL DROP TABLE dbo.Livro;
IF OBJECT_ID('dbo.Autor', 'U') IS NOT NULL DROP TABLE dbo.Autor;
IF OBJECT_ID('dbo.Assunto', 'U') IS NOT NULL DROP TABLE dbo.Assunto;
GO

CREATE TABLE dbo.Livro
(
  Codl INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Livro PRIMARY KEY,
  Titulo VARCHAR(40) NOT NULL,
  Editora VARCHAR(40) NOT NULL,
  Edicao INT NOT NULL,
  AnoPublicacao CHAR(4) NOT NULL,
  Valor DECIMAL(10,2) NOT NULL CONSTRAINT DF_Livro_Valor DEFAULT (0),
  DataCriacao DATETIME2(0) NOT NULL CONSTRAINT DF_Livro_DataCriacao DEFAULT SYSUTCDATETIME(),
  DataAtualizacao DATETIME2(0) NULL
);
GO

CREATE TABLE dbo.Autor
(
  CodAu INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Autor PRIMARY KEY,
  Nome VARCHAR(40) NOT NULL
);
GO

CREATE TABLE dbo.Assunto
(
  codAs INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Assunto PRIMARY KEY,
  Descricao VARCHAR(20) NOT NULL
);
GO

CREATE TABLE dbo.Livro_Autor
(
  Livro_Codl INT NOT NULL,
  Autor_CodAu INT NOT NULL,
  CONSTRAINT PK_Livro_Autor PRIMARY KEY (Livro_Codl, Autor_CodAu),
  CONSTRAINT FK_Livro_Autor_Livro FOREIGN KEY (Livro_Codl) REFERENCES dbo.Livro (Codl) ON DELETE CASCADE,
  CONSTRAINT FK_Livro_Autor_Autor FOREIGN KEY (Autor_CodAu) REFERENCES dbo.Autor (CodAu) ON DELETE CASCADE
);
GO

CREATE TABLE dbo.Livro_Assunto
(
  Livro_Codl INT NOT NULL,
  Assunto_codAs INT NOT NULL,
  CONSTRAINT PK_Livro_Assunto PRIMARY KEY (Livro_Codl, Assunto_codAs),
  CONSTRAINT FK_Livro_Assunto_Livro FOREIGN KEY (Livro_Codl) REFERENCES dbo.Livro (Codl) ON DELETE CASCADE,
  CONSTRAINT FK_Livro_Assunto_Assunto FOREIGN KEY (Assunto_codAs) REFERENCES dbo.Assunto (codAs) ON DELETE CASCADE
);
GO
