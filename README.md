# Conexão Solidária

Plataforma de microsserviços .NET 9 para gestão de ONG, desenvolvida para o Hackathon 9NETT.

## Arquitetura

A solução utiliza arquitetura de microsserviços com comunicação assíncrona via RabbitMQ:

- **AuthService**: Autenticação JWT com RBAC (GestorONG, Doador)
- **CampaignService**: Gestão de campanhas de arrecadação
- **DonationService**: Registro de doações
- **DonationWorker**: Processamento assíncrono de doações
- **TransparencyService**: API pública de transparência

## Pré-requisitos

- .NET 9.0 SDK
- Docker e Docker Compose
- Kubernetes (Minikube, Kind ou Docker Desktop K8s)
- kubectl

## Setup Local com Docker Compose

### 1. Clonar o repositório
```bash
git clone <repositorio>
cd ConexaoSolidaria
```

### 2. Iniciar infraestrutura
```bash
docker-compose up -d
```

### 3. Criar bancos de dados
```bash
# Aguardar 2-3 minutos para os SQL Servers iniciarem
./scripts/create-databases.sh
```

### 4. Executar migrations
```bash
# AuthService
cd src/AuthService/AuthService
dotnet ef migrations add InitialCreate
dotnet ef database update

# CampaignService
cd ../../CampaignService/CampaignService
dotnet ef migrations add InitialCreate
dotnet ef database update

# DonationService
cd ../../DonationService/DonationService
dotnet ef migrations add InitialCreate
dotnet ef database update

# TransparencyService
cd ../../TransparencyService/TransparencyService
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Testar os serviços
```bash
# Swagger APIs
curl http://localhost:5001/swagger  # AuthService
curl http://localhost:5002/swagger  # CampaignService
curl http://localhost:5003/swagger  # DonationService
curl http://localhost:5004/swagger  # TransparencyService

# RabbitMQ Management
# http://localhost:15672 (admin/admin123)
```

## Setup com Kubernetes

### 1. Aplicar ConfigMaps e Secrets
```bash
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secrets.yaml
```

### 2. Aplicar Deployments e Services
```bash
kubectl apply -f k8s/auth-service-deployment.yaml
kubectl apply -f k8s/campaign-service-deployment.yaml
kubectl apply -f k8s/donation-service-deployment.yaml
kubectl apply -f k8s/donation-worker-deployment.yaml
kubectl apply -f k8s/transparency-service-deployment.yaml
```

### 3. Verificar status
```bash
kubectl get pods
kubectl get services
```

### 4. Criar bancos de dados
```bash
# Conectar ao SQL Server via kubectl exec
kubectl exec -it <pod-sqlserver-auth> -- /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "CREATE DATABASE AuthDb"
kubectl exec -it <pod-sqlserver-campaign> -- /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "CREATE DATABASE CampaignDb"
kubectl exec -it <pod-sqlserver-donation> -- /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -Q "CREATE DATABASE DonationDb"
```

### 5. Executar migrations
```bash
# Para cada serviço, executar migrations via kubectl exec
kubectl exec -it <pod-auth-service> -- dotnet ef database update
kubectl exec -it <pod-campaign-service> -- dotnet ef database update
kubectl exec -it <pod-donation-service> -- dotnet ef database update
kubectl exec -it <pod-transparency-service> -- dotnet ef database update
```

## API Endpoints

### AuthService
- `POST /api/auth/register` - Cadastro de doador
- `POST /api/auth/login` - Login e geração de token JWT
- `POST /api/auth/promote-to-gestor` - Promover usuário a GestorONG (requer role GestorONG)

### CampaignService (Requer role GestorONG)
- `GET /api/campaign` - Listar todas as campanhas
- `GET /api/campaign/{id}` - Obter campanha por ID
- `POST /api/campaign` - Criar nova campanha
- `PUT /api/campaign/{id}` - Atualizar campanha
- `DELETE /api/campaign/{id}` - Deletar campanha

### DonationService (Requer autenticação)
- `POST /api/donation` - Registrar nova doação
- `GET /api/donation` - Listar todas as doações

### TransparencyService (Público)
- `GET /api/transparency` - Listar campanhas ativas com valores arrecadados

### Health Endpoints
- `GET /api/health` - Health check de todos os serviços

## Exemplo de Uso

### 1. Cadastrar doador
```bash
curl -X POST http://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "nomeCompleto": "João Silva",
    "email": "joao@example.com",
    "cpf": "12345678901",
    "senha": "Senha123!"
  }'
```

### 2. Login
```bash
curl -X POST http://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@example.com",
    "senha": "Senha123!"
  }'
```

### 3. Criar campanha (como GestorONG)
```bash
curl -X POST http://localhost:5002/api/campaign \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token-jwt>" \
  -d '{
    "titulo": "Campanha de Natal",
    "descricao": "Arrecadação para crianças carentes",
    "dataInicio": "2024-12-01T00:00:00",
    "dataFim": "2024-12-31T23:59:59",
    "metaFinanceira": 10000.00,
    "status": "Ativa"
  }'
```

### 4. Fazer doação
```bash
curl -X POST http://localhost:5003/api/donation \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <token-jwt>" \
  -d '{
    "campanhaId": 1,
    "valor": 100.00
  }'
```

### 5. Consultar transparência
```bash
curl http://localhost:5004/api/transparency
```

## CI/CD

O pipeline CI/CD é acionado automaticamente em cada push para as branches `main` ou `develop`:

1. **Restore**: Restaura as dependências do projeto
2. **Build**: Compila a solução em modo Release
3. **Test**: Executa os testes unitários
4. **Docker Build**: Constrói as imagens Docker de todos os serviços
5. **Docker Push**: Publica as imagens no Docker Hub (apenas na branch main)

## Observabilidade

### Health Checks
Todos os serviços expõem o endpoint `/api/health` para monitoramento de saúde.

### Grafana Dashboard
Configure o Grafana para monitorar:
- Métricas de CPU e memória dos pods Kubernetes
- Taxa de requisições HTTP por serviço
- Status dos health checks

## Estrutura do Projeto

```
ConexaoSolidaria/
├── src/
│   ├── AuthService/              # Serviço de autenticação
│   ├── CampaignService/          # Serviço de campanhas
│   ├── DonationService/          # Serviço de doações
│   ├── DonationWorker/           # Worker assíncrono
│   ├── TransparencyService/     # Serviço de transparência
│   └── SharedModels/             # Models compartilhados
├── docker/                      # Dockerfiles
├── k8s/                         # Manifestos Kubernetes
├── scripts/                     # Scripts utilitários
├── .github/workflows/           # CI/CD workflows
└── docker-compose.yml           # Orquestração local
```

## Tecnologias

- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server 2022
- RabbitMQ
- Docker
- Kubernetes
- GitHub Actions
- Serilog (Logging)
- JWT Authentication
- BCrypt (Hash de senhas)

## Licença

Este projeto foi desenvolvido para o Hackathon 9NETT da FIAP.
