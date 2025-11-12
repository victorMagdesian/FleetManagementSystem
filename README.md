# FleetManager 🚗

Sistema de gestão de frota e manutenção preventiva para empresas de transporte e turismo, construído com ASP.NET Core 8 seguindo os princípios de Clean Architecture.

## 📑 Índice de Documentação

- **[Guia de Início Rápido](#-guia-de-início-rápido)** - Comece aqui para rodar o projeto
- **[Arquitetura](#-arquitetura)** - Entenda a estrutura do projeto
- **[Tecnologias](#-tecnologias)** - Stack tecnológico utilizado
- **[API Endpoints](docs/API.md)** - Documentação completa da API REST
- **[Banco de Dados](docs/DATABASE.md)** - Schema e migrações
- **[Docker](DOCKER.md)** - Configuração e uso do Docker Compose
- **[Redis Cache](REDIS_SETUP.md)** - Configuração e uso do cache
- **[Testes](docs/TESTING.md)** - Guia de testes unitários e de integração
- **[Configuração](docs/CONFIGURATION.md)** - Variáveis de ambiente e appsettings
- **[Background Jobs](docs/JOBS.md)** - Quartz.NET e tarefas agendadas

## 🚀 Guia de Início Rápido

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (recomendado)
- [Git](https://git-scm.com/)

### Executar o Projeto (Docker Compose)

1. Clone o repositório:
```bash
git clone <repository-url>
cd FleetManager
```

2. Inicie os serviços de infraestrutura:
```bash
docker-compose up -d
```

3. Execute as migrações do banco de dados:
```bash
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
```

4. Execute a API:
```bash
dotnet run --project src/FleetManager.Api
```

5. Acesse a aplicação:
- **API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Seq (Logs)**: http://localhost:5341

### Executar Testes

```bash
# Todos os testes
dotnet test

# Apenas testes unitários
dotnet test --filter "FullyQualifiedName~Unit"

# Apenas testes de integração
dotnet test --filter "FullyQualifiedName~Integration"
```

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture** com separação clara entre camadas:

```
/src
├── FleetManager.Api              # 🌐 Presentation Layer (ASP.NET Core Web API)
│   ├── Controllers/              # REST API Controllers
│   ├── Middleware/               # Custom middleware (Exception handling)
│   └── Program.cs                # Application entry point & DI configuration
│
├── FleetManager.Application      # 💼 Application Layer
│   ├── DTOs/                     # Data Transfer Objects (Request/Response)
│   ├── Interfaces/               # Application service interfaces
│   ├── Services/                 # Application services (use cases)
│   └── Mappings/                 # AutoMapper profiles
│
├── FleetManager.Domain           # 🎯 Domain Layer
│   ├── Entities/                 # Domain entities (Vehicle, Driver, Trip, etc.)
│   ├── Enums/                    # Domain enumerations (VehicleStatus)
│   ├── Exceptions/               # Custom domain exceptions
│   └── Interfaces/               # Repository interfaces
│
├── FleetManager.Infrastructure   # 🔧 Infrastructure Layer
│   ├── Data/                     # EF Core DbContext & Configurations
│   ├── Repositories/             # Repository implementations
│   ├── Jobs/                     # Quartz.NET background jobs
│   └── Cache/                    # Redis cache implementation
│
└── FleetManager.Tests            # 🧪 Test Layer
    ├── Unit/                     # Unit tests (Domain & Application)
    └── Integration/              # Integration tests (API & Database)
```

### Princípios de Design

- **Clean Architecture**: Separação de responsabilidades e independência de frameworks
- **Domain-Driven Design**: Entidades ricas com lógica de negócio encapsulada
- **Repository Pattern**: Abstração da camada de acesso a dados
- **Dependency Injection**: Inversão de controle para todas as dependências
- **SOLID Principles**: Código mantível e testável

## 🛠️ Tecnologias

### Backend
- **ASP.NET Core 8** - Framework web
- **Entity Framework Core 8** - ORM para acesso a dados
- **SQL Server 2022** - Banco de dados relacional
- **Redis 7** - Cache distribuído
- **Quartz.NET 3.13** - Agendamento de tarefas
- **Serilog 8.0** - Logging estruturado
- **AutoMapper 13.0** - Mapeamento objeto-objeto

### Testing
- **xUnit** - Framework de testes
- **Moq 4.20** - Biblioteca de mocking
- **Microsoft.AspNetCore.Mvc.Testing** - Testes de integração

### DevOps
- **Docker & Docker Compose** - Containerização
- **Seq** - Agregação e visualização de logs

## 📦 Pacotes NuGet

### FleetManager.Domain
- Nenhuma dependência externa (camada pura de domínio)

### FleetManager.Application
- AutoMapper 13.0.1

### FleetManager.Infrastructure
- Microsoft.EntityFrameworkCore 8.0.11
- Microsoft.EntityFrameworkCore.SqlServer 8.0.11
- Microsoft.EntityFrameworkCore.Design 8.0.11
- Quartz 3.13.1
- Quartz.Extensions.Hosting 3.13.1
- StackExchange.Redis 2.8.16
- Serilog.AspNetCore 8.0.3

### FleetManager.Api
- Referências aos projetos Application e Infrastructure

### FleetManager.Tests
- xUnit (incluído no template)
- Moq 4.20.72
- Microsoft.AspNetCore.Mvc.Testing 8.0.11

## 🎯 Funcionalidades Principais

### Gestão de Veículos
- ✅ Cadastro, atualização e exclusão de veículos
- ✅ Consulta de veículos disponíveis
- ✅ Controle de status (Disponível, Em Uso, Em Manutenção)
- ✅ Rastreamento de quilometragem
- ✅ Cálculo automático de próxima manutenção

### Gestão de Condutores
- ✅ Cadastro, atualização e exclusão de condutores
- ✅ Consulta de condutores disponíveis
- ✅ Ativação/desativação de condutores
- ✅ Validação de CNH única

### Gestão de Manutenções
- ✅ Registro de manutenções realizadas
- ✅ Histórico de manutenções por veículo
- ✅ Alertas automáticos de manutenção próxima
- ✅ Atualização automática de datas de manutenção

### Gestão de Viagens
- ✅ Início e finalização de viagens
- ✅ Validação de disponibilidade de veículo e condutor
- ✅ Atualização automática de status e quilometragem
- ✅ Consulta de viagens ativas

### Recursos Técnicos
- ✅ Cache Redis para consultas frequentes
- ✅ Background job diário para verificação de manutenções
- ✅ Logging estruturado com Serilog e Seq
- ✅ Exception handling global
- ✅ Swagger/OpenAPI documentation
- ✅ Testes unitários e de integração

## 📚 Documentação Detalhada

### Para Desenvolvedores

- **[API Endpoints](docs/API.md)** - Documentação completa de todos os endpoints REST com exemplos de request/response
- **[Banco de Dados](docs/DATABASE.md)** - Schema do banco, migrações e comandos úteis
- **[Testes](docs/TESTING.md)** - Como escrever e executar testes unitários e de integração
- **[Configuração](docs/CONFIGURATION.md)** - Variáveis de ambiente e configurações do appsettings.json

### Para DevOps

- **[Docker](DOCKER.md)** - Como usar Docker Compose para desenvolvimento e produção
- **[Redis Cache](REDIS_SETUP.md)** - Configuração e monitoramento do cache Redis
- **[Background Jobs](docs/JOBS.md)** - Configuração e monitoramento de jobs Quartz.NET

## 🔧 Comandos Úteis

### Build e Execução

```bash
# Restaurar dependências
dotnet restore

# Build do projeto
dotnet build

# Build em modo Release
dotnet build --configuration Release

# Executar a API
dotnet run --project src/FleetManager.Api

# Executar com hot reload
dotnet watch run --project src/FleetManager.Api
```

### Banco de Dados

```bash
# Criar nova migração
dotnet ef migrations add <MigrationName> --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api

# Aplicar migrações
dotnet ef database update --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api

# Reverter última migração
dotnet ef migrations remove --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api

# Gerar script SQL
dotnet ef migrations script --project src/FleetManager.Infrastructure --startup-project src/FleetManager.Api
```

### Docker

```bash
# Iniciar todos os serviços
docker-compose up -d

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down

# Parar e remover volumes (limpar dados)
docker-compose down -v

# Rebuild da API
docker-compose build api
docker-compose up -d --force-recreate api
```

### Testes

```bash
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test /p:CollectCoverage=true

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~VehicleService"

# Executar com verbosidade
dotnet test --verbosity detailed
```

## 🌐 URLs de Acesso

Após iniciar a aplicação, os seguintes serviços estarão disponíveis:

| Serviço | URL | Descrição |
|---------|-----|-----------|
| API | http://localhost:5000 | API REST principal |
| Swagger UI | http://localhost:5000/swagger | Documentação interativa da API |
| SQL Server | localhost:1433 | Banco de dados (user: sa) |
| Redis | localhost:6379 | Cache distribuído |
| Seq | http://localhost:5341 | Visualização de logs |

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para mais detalhes.

## 📧 Contato

Para dúvidas ou sugestões, abra uma issue no repositório.
