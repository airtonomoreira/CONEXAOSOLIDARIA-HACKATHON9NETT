# Scripts de Migrations - Conexão Solidária

## Visão Geral

Este diretório contém scripts SQL manuais para criação das tabelas do banco de dados, uma alternativa ao uso do Entity Framework Migrations quando o dotnet-ef tool não está disponível.

## Scripts Disponíveis

1. **01-auth-service-initial.sql** - Criação das tabelas do AuthService (AuthDb)
   - Tabelas do ASP.NET Core Identity (AspNetUsers, AspNetRoles, etc.)
   - Tabela personalizada Doadores
   - Roles padrão (GestorONG, Doador)

2. **02-campaign-service-initial.sql** - Criação das tabelas do CampaignService (CampaignDb)
   - Tabela Campanhas
   - Constraints e índices

3. **03-donation-service-initial.sql** - Criação das tabelas do DonationService (DonationDb)
   - Tabela Doacoes
   - Constraints e índices

4. **04-transparency-service-initial.sql** - Criação das tabelas do TransparencyService (TransparencyDb)
   - Tabela Campanhas (read-only replica)
   - Índices para consultas públicas

## Como Executar

### Via SQL Server Management Studio (SSMS)

1. Abra o SSMS e conecte-se ao servidor SQL Server
2. Selecione o database correspondente (AuthDb, CampaignDb, DonationDb, TransparencyDb)
3. Abra o arquivo SQL correspondente
4. Execute o script (F5 ou botão Execute)

### Via sqlcmd (Command Line)

```bash
# AuthService
sqlcmd -S localhost,1433 -U sa -P YourStrong@Passw0rd -d AuthDb -i scripts/migrations/01-auth-service-initial.sql

# CampaignService
sqlcmd -S localhost,1433 -U sa -P YourStrong@Passw0rd -d CampaignDb -i scripts/migrations/02-campaign-service-initial.sql

# DonationService
sqlcmd -S localhost,1433 -U sa -P YourStrong@Passw0rd -d DonationDb -i scripts/migrations/03-donation-service-initial.sql

# TransparencyService
sqlcmd -S localhost,1433 -U sa -P YourStrong@Passw0rd -d TransparencyDb -i scripts/migrations/04-transparency-service-initial.sql
```

### Via Docker Compose

Os scripts serão executados automaticamente quando o SQL Server for inicializado se você adicionar os scripts ao volume do container.

### Via Kubernetes

Adicione os scripts como ConfigMaps ou init containers para executar durante o deploy.

## Estrutura das Tabelas

### AuthDb
- AspNetUsers (Identity)
- AspNetRoles (Identity)
- AspNetUserClaims (Identity)
- AspNetUserLogins (Identity)
- AspNetUserRoles (Identity)
- AspNetUserTokens (Identity)
- AspNetRoleClaims (Identity)
- Doadores (Custom)

### CampaignDb
- Campanhas

### DonationDb
- Doacoes

### TransparencyDb
- Campanhas (read-only)

## Notas Importantes

- Os scripts usam `IF NOT EXISTS` para permitir execução múltipla segura
- Constraints CHECK garantem integridade dos dados
- Índices foram criados para otimizar consultas comuns
- O DonationWorker compartilha o DonationDb com o DonationService

## Alternativa: Entity Framework Migrations

Se preferir usar EF Core migrations nativos, instale o dotnet-ef tool:

```bash
dotnet tool install --global dotnet-ef
```

E execute as migrations para cada serviço:

```bash
# AuthService
dotnet ef migrations add InitialCreate --project src/AuthService/AuthService/AuthService.csproj --startup-project src/AuthService/AuthService/AuthService.csproj

# CampaignService
dotnet ef migrations add InitialCreate --project src/CampaignService/CampaignService/CampaignService.csproj --startup-project src/CampaignService/CampaignService/CampaignService.csproj

# DonationService
dotnet ef migrations add InitialCreate --project src/DonationService/DonationService/DonationService.csproj --startup-project src/DonationService/DonationService/DonationService.csproj

# TransparencyService
dotnet ef migrations add InitialCreate --project src/TransparencyService/TransparencyService/TransparencyService.csproj --startup-project src/TransparencyService/TransparencyService/TransparencyService.csproj
```

## Rollback

Para reverter as mudanças, você pode:

1. Dropar as tabelas manualmente:
```sql
DROP TABLE IF EXISTS Doadores;
DROP TABLE IF EXISTS AspNetRoleClaims;
DROP TABLE IF EXISTS AspNetUserTokens;
DROP TABLE IF EXISTS AspNetUserRoles;
DROP TABLE IF EXISTS AspNetUserLogins;
DROP TABLE IF EXISTS AspNetUserClaims;
DROP TABLE IF EXISTS AspNetUsers;
DROP TABLE IF EXISTS AspNetRoles;
```

2. Ou recriar o database do zero:
```sql
DROP DATABASE [NomeDoDatabase];
CREATE DATABASE [NomeDoDatabase];
```
