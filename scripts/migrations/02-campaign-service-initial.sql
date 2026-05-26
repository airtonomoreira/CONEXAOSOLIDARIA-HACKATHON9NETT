-- Migration inicial para CampaignService - CampaignDb
-- Database: CampaignDb
-- SQL Server

-- Tabela Campanhas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Campanhas')
BEGIN
    CREATE TABLE Campanhas (
        Id INT NOT NULL IDENTITY PRIMARY KEY,
        Titulo NVARCHAR(200) NOT NULL,
        Descricao NVARCHAR(MAX) NOT NULL,
        DataInicio DATETIME2 NOT NULL DEFAULT GETDATE(),
        DataFim DATETIME2 NOT NULL,
        MetaFinanceira DECIMAL(18,2) NOT NULL,
        ValorArrecadado DECIMAL(18,2) NOT NULL DEFAULT 0.00,
        Status NVARCHAR(20) NOT NULL DEFAULT 'Ativa',
        CONSTRAINT CK_Campanhas_Status CHECK (Status IN ('Ativa', 'Concluida', 'Cancelada')),
        CONSTRAINT CK_Campanhas_DataFim CHECK (DataFim > DataInicio),
        CONSTRAINT CK_Campanhas_MetaFinanceira CHECK (MetaFinanceira > 0),
        CONSTRAINT CK_Campanhas_ValorArrecadado CHECK (ValorArrecadado >= 0)
    );
    
    CREATE INDEX IX_Campanhas_Status ON Campanhas (Status);
    CREATE INDEX IX_Campanhas_DataInicio ON Campanhas (DataInicio);
    CREATE INDEX IX_Campanhas_DataFim ON Campanhas (DataFim);
END

PRINT 'Migration inicial do CampaignService concluída com sucesso.';
