-- Migration inicial para DonationService - DonationDb
-- Database: DonationDb
-- SQL Server

-- Tabela Doacoes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Doacoes')
BEGIN
    CREATE TABLE Doacoes (
        Id INT NOT NULL IDENTITY PRIMARY KEY,
        CampanhaId INT NOT NULL,
        DoadorId INT NOT NULL,
        Valor DECIMAL(18,2) NOT NULL,
        DataDoacao DATETIME2 NOT NULL DEFAULT GETDATE(),
        Status NVARCHAR(20) NOT NULL DEFAULT 'Pendente',
        CONSTRAINT CK_Doacoes_Status CHECK (Status IN ('Pendente', 'Processada', 'Cancelada')),
        CONSTRAINT CK_Doacoes_Valor CHECK (Valor > 0)
    );
    
    CREATE INDEX IX_Doacoes_CampanhaId ON Doacoes (CampanhaId);
    CREATE INDEX IX_Doacoes_DoadorId ON Doacoes (DoadorId);
    CREATE INDEX IX_Doacoes_Status ON Doacoes (Status);
    CREATE INDEX IX_Doacoes_DataDoacao ON Doacoes (DataDoacao);
END

PRINT 'Migration inicial do DonationService concluída com sucesso.';
