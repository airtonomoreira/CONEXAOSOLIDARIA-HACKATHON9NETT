# Justificativa da Escolha do SQL Server - Conexão Solidária

## Introdução

Este documento justifica a escolha do Microsoft SQL Server como banco de dados relacional para o projeto Conexão Solidária, uma plataforma de doações para campanhas de caridade.

## Requisitos do Projeto

O Conexão Solidária possui os seguintes requisitos de dados:

- **Consistência de dados**: Transações financeiras exigem ACID compliance
- **Relacionamentos complexos**: Doadores, campanhas e doações estão interligados
- **Integridade referencial**: Garantir que doações estejam vinculadas a doadores e campanhas válidos
- **Consultas complexas**: Relatórios de transparência com agregações e filtros
- **Escalabilidade**: Suportar crescimento do número de doações e usuários
- **Segurança**: Proteção de dados sensíveis (CPF, informações financeiras)

## Por que SQL Server?

### 1. ACID Compliance e Consistência de Transações

**Justificativa**: Doações envolvem transações financeiras que exigem consistência absoluta.

**Benefícios do SQL Server**:
- Suporte completo a transações ACID (Atomicity, Consistency, Isolation, Durability)
- Rollback automático em caso de falhas
- Isolamento de transações configurável (Read Committed, Serializable, etc.)
- Garantia de integridade dos dados financeiros

**Alternativas consideradas**:
- MongoDB: Não oferece ACID completo em todos os cenários
- PostgreSQL: Excelente alternativa, mas SQL Server tem melhor integração com ecossistema Microsoft

### 2. Integridade Referencial e Constraints

**Justificativa**: O modelo de dados possui relacionamentos estritos que devem ser mantidos.

**Benefícios do SQL Server**:
- Foreign Keys com cascading deletes/updates
- Check constraints para validação de dados
- Unique constraints para garantir unicidade (email, CPF)
- Default values e computed columns

**Exemplo no projeto**:
```csharp
// AuthDbContext - Garantia de email único
entity.HasIndex(e => e.Email).IsUnique();

// DonationDbContext - Valor obrigatório e preciso
entity.Property(e => e.Valor).IsRequired().HasPrecision(18, 2);
```

### 3. Performance em Consultas Complexas

**Justificativa**: O serviço de transparência requer consultas agregadas complexas.

**Benefícios do SQL Server**:
- Query Optimizer avançado
- Índices clusterizados e não-clusterizados
- Indexed Views para consultas frequentes
- Columnstore indexes para analytics
- In-Memory OLTP para transações de alta performance

**Caso de uso**: Relatório de transparência com:
- Sum de doações por campanha
- Contagem de doadores por período
- Média de valores doados
- Filtragem por status e período

### 4. Segurança e Compliance

**Justificativa**: Dados pessoais (CPF, email) e financeiros exigem proteção robusta.

**Benefícios do SQL Server**:
- Transparent Data Encryption (TDE) para criptografia em repouso
- Always Encrypted para criptografia de colunas sensíveis
- Row-Level Security para controle de acesso granular
- Auditing completo de operações
- Compliance com LGPD, PCI-DSS e SOC 2

**Implementação no projeto**:
- Senhas hashadas com BCrypt
- Dados sensíveis criptografados
- Logs de auditoria para todas as operações

### 5. Integração com Ecossistema .NET

**Justificativa**: O projeto utiliza .NET 9 e Entity Framework Core.

**Benefícios do SQL Server**:
- Suporte nativo e otimizado no Entity Framework Core
- Migrations automatizadas com EF Core
- LINQ to SQL com tradução eficiente de queries
- Suporte a async/await para operações de banco
- Tooling robusto (SSMS, Azure Data Studio)

**Exemplo**:
```csharp
// Integração nativa com EF Core
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 6. Escalabilidade e Alta Disponibilidade

**Justificativa**: O sistema deve suportar crescimento e manter disponibilidade.

**Benefícios do SQL Server**:
- Always On Availability Groups para HA
- Database mirroring
- Replicação geográfica
- Sharding e partitioning
- Auto-tuning e automatic index management

**Arquitetura do projeto**:
- Cada microserviço tem seu próprio banco (Database per Service)
- Escalabilidade horizontal via Kubernetes
- Possibilidade de migrar para Azure SQL Server sem mudanças no código

### 7. Suporte a JSON e Semi-Structured Data

**Justificativa**: Flexibilidade para metadados e extensões futuras.

**Benefícios do SQL Server**:
- Tipo de dados JSON nativo
- Funções para query e manipulação de JSON
- Índices em propriedades JSON
- Hybrid approach: relacional + JSON quando necessário

**Caso de uso**: Metadados de campanhas, configurações flexíveis

### 8. Custo-Benefício e TCO

**Justificativa**: Balanceamento entre custo e funcionalidade.

**Análise**:
- **SQL Server Express**: Gratuito para até 10GB (suficiente para MVP)
- **SQL Server Standard**: Custo moderado para produção
- **Azure SQL Server**: Pay-as-you-go, escalável, sem overhead de infraestrutura
- **Suporte da Microsoft**: Documentação extensa, comunidade ativa

**Alternativas não escolhidas**:
- PostgreSQL: Excelente, mas menor integração com ecossistema Microsoft
- Oracle: Custo elevado, complexidade desnecessária
- MySQL: Menos recursos enterprise que SQL Server

## Arquitetura de Dados do Projeto

### Database per Service Pattern

O projeto adota o padrão "Database per Service" para desacoplamento:

| Serviço | Database | Tabelas Principais |
|---------|----------|-------------------|
| AuthService | AuthDb | AspNetUsers, AspNetRoles, Doadores |
| CampaignService | CampaignDb | Campanhas |
| DonationService | DonationDb | Doações |
| TransparencyService | TransparencyDb | Campanhas (read replica) |
| DonationWorker | DonationDb | Doações (leitura/escrita) |

### Vantagens desta Arquitetura

1. **Desacoplamento**: Cada serviço evolui independentemente
2. **Escalabilidade**: Cada banco pode escalar conforme necessidade
3. **Resiliência**: Falha em um banco não afeta outros serviços
4. **Performance**: Queries otimizadas para cada domínio
5. **Segurança**: Princípio de menor privilégio por serviço

## Conclusão

A escolha do SQL Server para o projeto Conexão Solidária é justificada por:

1. **Requisitos de consistência**: Transações financeiras exigem ACID
2. **Integridade referencial**: Relacionamentos complexos entre entidades
3. **Performance**: Consultas complexas de relatórios de transparência
4. **Segurança**: Proteção de dados sensíveis e compliance
5. **Integração**: Suporte nativo com .NET e Entity Framework Core
6. **Escalabilidade**: Suporte a crescimento e alta disponibilidade
7. **Custo-benefício**: SQL Server Express gratuito para MVP, Azure SQL para produção

O SQL Server atende perfeitamente aos requisitos do projeto enquanto oferece caminho claro para evolução e escalabilidade, mantendo baixo custo inicial e alta qualidade técnica.

## Referências

- [Microsoft SQL Server Documentation](https://docs.microsoft.com/en-us/sql/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [ACID Transactions](https://en.wikipedia.org/wiki/ACID)
- [Database per Service Pattern](https://microservices.io/patterns/data/database-per-service.html)
