# FleetManager

Sistema de gestão de frota e manutenção preventiva para empresas de transporte e turismo.

## Arquitetura

O projeto segue os princípios de Clean Architecture com separação clara entre camadas:

```
/src
├── FleetManager.Api              # Presentation Layer (ASP.NET Core Web API)
│   ├── Controllers/              # REST API Controllers
│   └── Middleware/               # Custom middleware
│
├── FleetManager.Application      # Application Layer
│   ├── DTOs/                     # Data Transfer Objects
│   ├── Interfaces/               # Application service interfaces
│   ├── Services/                 # Application services (use cases)
│   └── Mappings/                 # AutoMapper profiles
│
├── FleetManager.Domain           # Domain Layer
│   ├── Entities/                 # Domain entities
│   ├── Enums/                    # Domain enumerations
│   └── Interfaces/               # Repository interfaces
│
├── FleetManager.Infrastructure   # Infrastructure Layer
│   ├── Data/                     # EF Core DbContext
│   ├── Repositories/             # Repository implementations
│   ├── Jobs/                     # Quartz.NET jobs
│   └── Cache/                    # Redis cache implementation
│
└── FleetManager.Tests            # Test Layer
    ├── Unit/                     # Unit tests
    └── Integration/              # Integration tests
```

## Tecnologias

- **Backend**: ASP.NET Core 8 Web API
- **Database**: SQL Server with Entity Framework Core 8
- **Background Jobs**: Quartz.NET 3.13.1
- **Caching**: Redis (StackExchange.Redis 2.8.16)
- **Logging**: Serilog 3.1.1
- **Mapping**: AutoMapper 13.0.1
- **Testing**: xUnit + Moq 4.20.72

## Pacotes NuGet Instalados

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

## Referências entre Projetos

```
FleetManager.Api
  └── FleetManager.Application
  └── FleetManager.Infrastructure

FleetManager.Application
  └── FleetManager.Domain

FleetManager.Infrastructure
  └── FleetManager.Domain
  └── FleetManager.Application

FleetManager.Tests
  └── FleetManager.Domain
  └── FleetManager.Application
  └── FleetManager.Infrastructure
  └── FleetManager.Api
```

## Como Executar

### Pré-requisitos
- .NET 8 SDK
- Docker e Docker Compose (recomendado)

### Opção 1: Usando Docker Compose (Recomendado)

1. Iniciar os containers (SQL Server + Redis):
```bash
docker-compose up -d
```

2. Verificar se os containers estão rodando:
```bash
docker-compose ps
```

3. Executar a API:
```bash
dotnet run --project src/FleetManager.Api
```

4. Parar os containers:
```bash
docker-compose down
```

5. Parar e remover volumes (limpar dados):
```bash
docker-compose down -v
```

### Opção 2: Instalação Local

Se preferir instalar SQL Server e Redis localmente:
- SQL Server 2022 ou superior
- Redis 7 ou superior

### Build
```bash
dotnet build
```

### Executar Testes
```bash
dotnet test
```

### Executar API
```bash
dotnet run --project src/FleetManager.Api
```

A API estará disponível em:
- HTTP: http://localhost:5000
- HTTPS: https://localhost:5001
- Swagger: http://localhost:5000/swagger

## Próximos Passos

1. Implementar entidades do domínio (Vehicle, Driver, MaintenanceRecord, Trip)
2. Criar repositórios e DbContext
3. Implementar serviços da camada de aplicação
4. Criar controllers da API
5. Configurar Quartz.NET para jobs de manutenção
6. Adicionar testes unitários e de integração
