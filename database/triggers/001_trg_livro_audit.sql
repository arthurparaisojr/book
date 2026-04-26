USE BookDb;
GO

CREATE OR ALTER TRIGGER dbo.trg_Livro_Audit
ON dbo.Livro
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
  SET NOCOUNT ON;

  IF EXISTS (SELECT 1 FROM inserted) AND EXISTS (SELECT 1 FROM deleted)
  BEGIN
    INSERT INTO dbo.Livro_Audit
    (
      Acao,
      Codl,
      TituloAnterior,
      TituloNovo,
      ValorAnterior,
      ValorNovo
    )
    SELECT
      'UPDATE',
      d.Codl,
      d.Titulo,
      i.Titulo,
      d.Valor,
      i.Valor
    FROM deleted d
    INNER JOIN inserted i
      ON i.Codl = d.Codl;
  END
  ELSE IF EXISTS (SELECT 1 FROM inserted)
  BEGIN
    INSERT INTO dbo.Livro_Audit
    (
      Acao,
      Codl,
      TituloAnterior,
      TituloNovo,
      ValorAnterior,
      ValorNovo
    )
    SELECT
      'INSERT',
      i.Codl,
      NULL,
      i.Titulo,
      NULL,
      i.Valor
    FROM inserted i;
  END
  ELSE
  BEGIN
    INSERT INTO dbo.Livro_Audit
    (
      Acao,
      Codl,
      TituloAnterior,
      TituloNovo,
      ValorAnterior,
      ValorNovo
    )
    SELECT
      'DELETE',
      d.Codl,
      d.Titulo,
      NULL,
      d.Valor,
      NULL
    FROM deleted d;
  END
END;
GO
