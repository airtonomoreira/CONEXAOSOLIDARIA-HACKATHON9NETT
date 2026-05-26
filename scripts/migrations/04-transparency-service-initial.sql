-- Migration inicial para TransparencyService - TransparencyDb
-- Database: TransparencyDb
-- SQL Server

-- Tabela Campanhas (read-only replica)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Campanhas')
BEGIN
    CREATE TABLE Campanhas (
        Id INT NOT NULL PRIMARY KEY,
        Titulo NVARCHAR(200) NOT NULL,
        Descricao NVARCHAR(MAX) NOT NULL,
        DataInicio DATETIME2 NOT NULL,
        DataFim DATETIME2 NOT NULL,
        MetaFinanceira DECIMAL(18,2) NOT NULL,
        ValorArrecadado DECIMAL(18,2) NOT NULL,
        Status NVARCHAR(20) NOT NULL
    );
    
    CREATE INDEX IX_Campanhas_Status ON Campanhas (Status);
    CREATE INDEX IX_Campanhas_DataInicio ON Campanhas (DataInicio);
END

PRINT 'Migration inicial do TransparencyService concluída com sucesso.';
