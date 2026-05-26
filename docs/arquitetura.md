# Diagrama de Arquitetura - Conexão Solidária

## Visão Geral da Arquitetura

```mermaid
graph TB
    subgraph "Frontend / Clientes"
        Client[Aplicação Web / Mobile]
    end

    subgraph "API Gateway / Ingress"
        Ingress[Kubernetes Ingress]
    end

    subgraph "Camada de Serviços - Kubernetes"
        AuthService[AuthService<br/>Autenticação & Autorização]
        CampaignService[CampaignService<br/>Gerenciamento de Campanhas]
        DonationService[DonationService<br/>Processamento de Doações]
        TransparencyService[TransparencyService<br/>Transparência & Relatórios]
        DonationWorker[DonationWorker<br/>Worker Assíncrono]
    end

    subgraph "Message Broker"
        RabbitMQ[RabbitMQ<br/>Message Queue]
    end

    subgraph "Camada de Dados"
        AuthDB[(SQL Server<br/>AuthDb)]
        CampaignDB[(SQL Server<br/>CampaignDb)]
        DonationDB[(SQL Server<br/>DonationDb)]
        TransparencyDB[(SQL Server<br/>TransparencyDb)]
    end

    subgraph "Observabilidade"
        Prometheus[Prometheus<br/>Metrics]
        Grafana[Grafana<br/>Dashboards]
        Loki[Loki<br/>Logs]
    end

    Client --> Ingress
    Ingress --> AuthService
    Ingress --> CampaignService
    Ingress --> DonationService
    Ingress --> TransparencyService

    AuthService --> AuthDB
    CampaignService --> CampaignDB
    DonationService --> DonationDB
    TransparencyService --> TransparencyDB

    DonationService --> RabbitMQ
    RabbitMQ --> DonationWorker
    DonationWorker --> DonationDB

    AuthService -.-> Prometheus
    CampaignService -.-> Prometheus
    DonationService -.-> Prometheus
    TransparencyService -.-> Prometheus
    DonationWorker -.-> Prometheus

    Prometheus --> Grafana

    AuthService -.-> Loki
    CampaignService -.-> Loki
    DonationService -.-> Loki
    TransparencyService -.-> Loki
    DonationWorker -.-> Loki

    style AuthService fill:#4CAF50,color:#fff
    style CampaignService fill:#2196F3,color:#fff
    style DonationService fill:#FF9800,color:#fff
    style TransparencyService fill:#9C27B0,color:#fff
    style DonationWorker fill:#F44336,color:#fff
    style RabbitMQ fill:#FF5722,color:#fff
    style AuthDB fill:#607D8B,color:#fff
    style CampaignDB fill:#607D8B,color:#fff
    style DonationDB fill:#607D8B,color:#fff
    style TransparencyDB fill:#607D8B,color:#fff
```

## Fluxo de Dados

### Fluxo de Autenticação
```mermaid
sequenceDiagram
    participant Client
    participant AuthService
    participant AuthDB
    
    Client->>AuthService: POST /api/auth/register
    AuthService->>AuthDB: Criar usuário e role
    AuthDB-->>AuthService: Usuário criado
    AuthService-->>Client: JWT Token
    
    Client->>AuthService: POST /api/auth/login
    AuthService->>AuthDB: Validar credenciais
    AuthDB-->>AuthService: Usuário válido
    AuthService-->>Client: JWT Token
```

### Fluxo de Doação
```mermaid
sequenceDiagram
    participant Client
    participant DonationService
    participant RabbitMQ
    participant DonationWorker
    participant DonationDB
    participant CampaignService
    
    Client->>DonationService: POST /api/donations
    DonationService->>DonationDB: Criar doação (status: Pendente)
    DonationDB-->>DonationService: Doação criada
    DonationService->>RabbitMQ: Publicar mensagem
    DonationService-->>Client: 202 Accepted
    
    RabbitMQ->>DonationWorker: Consumir mensagem
    DonationWorker->>DonationDB: Atualizar status para Processada
    DonationWorker->>CampaignService: Atualizar valor arrecadado
    CampaignService->>CampaignDB: Incrementar ValorArrecadado
    CampaignDB-->>CampaignService: Atualizado
    CampaignService-->>DonationWorker: Sucesso
    DonationWorker-->>RabbitMQ: ACK
```

## Componentes da Arquitetura

### Microserviços

1. **AuthService**
   - Responsabilidade: Autenticação e autorização
   - Tecnologias: ASP.NET Core, JWT, Identity, EF Core
   - Banco: AuthDb (SQL Server)
   - Endpoints: Login, Register, Health

2. **CampaignService**
   - Responsabilidade: Gerenciamento de campanhas de doação
   - Tecnologias: ASP.NET Core, EF Core, RabbitMQ
   - Banco: CampaignDb (SQL Server)
   - Endpoints: CRUD Campanhas, Health

3. **DonationService**
   - Responsabilidade: Processamento de doações
   - Tecnologias: ASP.NET Core, EF Core, RabbitMQ
   - Banco: DonationDb (SQL Server)
   - Endpoints: Criar doação, Health

4. **TransparencyService**
   - Responsabilidade: Relatórios de transparência
   - Tecnologias: ASP.NET Core, EF Core, JWT
   - Banco: TransparencyDb (SQL Server)
   - Endpoints: Consultar campanhas, Health

5. **DonationWorker**
   - Responsabilidade: Processamento assíncrono de doações
   - Tecnologias: .NET Worker Service, RabbitMQ, EF Core
   - Banco: DonationDb (SQL Server)
   - Função: Consumir mensagens do RabbitMQ e atualizar status

### Infraestrutura

- **Kubernetes**: Orquestração de containers
- **RabbitMQ**: Message broker para comunicação assíncrona
- **SQL Server**: Banco de dados relacional
- **Prometheus**: Coleta de métricas
- **Grafana**: Visualização de métricas
- **Serilog**: Logging estruturado

## Padrões Arquiteturais Utilizados

1. **Microserviços**: Cada serviço tem responsabilidade única
2. **Message Queue**: Desacoplamento via RabbitMQ
3. **Database per Service**: Cada serviço tem seu próprio banco
4. **API Gateway**: Ingress do Kubernetes como gateway
5. **Health Checks**: Monitoramento de saúde dos serviços
6. **Observabilidade**: Logs, métricas e tracing

## Deploy

### Local (Docker Compose)
```bash
docker-compose up -d
```

### Kubernetes
```bash
kubectl apply -f k8s/
```

## Portas e Endpoints

| Serviço | Porta Interna | Porta Externa | Health Check |
|---------|---------------|---------------|--------------|
| AuthService | 8080 | 30001 | /api/health |
| CampaignService | 8080 | 30002 | /api/health |
| DonationService | 8080 | 30003 | /api/health |
| TransparencyService | 8080 | 30004 | /api/health |
| RabbitMQ | 5672/15672 | 30005/30006 | - |
| SQL Server | 1433 | 30007 | - |
