# Requirements Document

## Introduction

O FleetManager é um sistema de gestão de frota e manutenção preventiva para empresas de transporte e turismo. O sistema permite controle de veículos, condutores, agendas de manutenção e disponibilidade de forma visual e automatizada, utilizando ASP.NET Core 8 para a API backend, Blazor Server para o frontend, SQL Server com EF Core 8 para persistência, e Quartz.NET para tarefas agendadas.

## Glossary

- **FleetManager**: O sistema completo de gestão de frota
- **Vehicle**: Entidade que representa um veículo da frota
- **Driver**: Entidade que representa um condutor autorizado
- **MaintenanceRecord**: Registro histórico de manutenção realizada em um veículo
- **Trip**: Entidade que representa uma viagem realizada por um veículo e condutor
- **MaintenanceCheckJob**: Tarefa agendada que verifica manutenções próximas
- **API**: Interface REST para comunicação com o sistema
- **Repository**: Camada de acesso a dados seguindo o padrão Repository
- **Status**: Estado atual de um veículo (Available, InMaintenance, InUse)

## Requirements

### Requirement 1

**User Story:** Como gestor de frota, eu quero cadastrar e gerenciar veículos no sistema, para que eu possa manter um registro atualizado de toda a frota disponível.

#### Acceptance Criteria

1. THE FleetManager SHALL provide REST endpoints to create, read, update, and delete Vehicle records
2. WHEN a Vehicle is created, THE FleetManager SHALL validate that the Plate field is unique and not empty
3. THE FleetManager SHALL store Vehicle data including Id, Plate, Model, Year, Mileage, LastMaintenanceDate, NextMaintenanceDate, and Status
4. WHEN a Vehicle is retrieved, THE FleetManager SHALL return all Vehicle attributes in the response
5. THE FleetManager SHALL persist Vehicle data to the SQL Server database using Entity Framework Core

### Requirement 2

**User Story:** Como gestor de frota, eu quero cadastrar e gerenciar condutores, para que eu possa controlar quem está autorizado a operar os veículos.

#### Acceptance Criteria

1. THE FleetManager SHALL provide REST endpoints to create, read, update, and delete Driver records
2. WHEN a Driver is created, THE FleetManager SHALL validate that the LicenseNumber field is unique and not empty
3. THE FleetManager SHALL store Driver data including Id, Name, LicenseNumber, Phone, and Active status
4. THE FleetManager SHALL allow filtering Drivers by Active status
5. WHEN a Driver is deactivated, THE FleetManager SHALL update the Active field to false without deleting the record

### Requirement 3

**User Story:** Como gestor de frota, eu quero registrar manutenções realizadas nos veículos, para que eu possa manter um histórico completo de manutenção de cada veículo.

#### Acceptance Criteria

1. THE FleetManager SHALL provide REST endpoints to create and read MaintenanceRecord entries
2. WHEN a MaintenanceRecord is created, THE FleetManager SHALL associate it with a valid VehicleId
3. THE FleetManager SHALL store MaintenanceRecord data including Id, VehicleId, Date, Description, and Cost
4. WHEN a MaintenanceRecord is created, THE FleetManager SHALL update the associated Vehicle LastMaintenanceDate field
5. THE FleetManager SHALL allow retrieval of all MaintenanceRecord entries for a specific Vehicle

### Requirement 4

**User Story:** Como gestor de frota, eu quero que o sistema calcule automaticamente a próxima data de manutenção, para que eu possa planejar manutenções preventivas.

#### Acceptance Criteria

1. WHEN a MaintenanceRecord is created, THE FleetManager SHALL calculate and update the Vehicle NextMaintenanceDate field
2. THE FleetManager SHALL calculate NextMaintenanceDate by adding a configurable interval to the LastMaintenanceDate
3. WHEN a Vehicle Mileage is updated, THE FleetManager SHALL recalculate NextMaintenanceDate if mileage-based maintenance is configured
4. THE FleetManager SHALL provide an endpoint to retrieve Vehicles with upcoming maintenance within a specified number of days

### Requirement 5

**User Story:** Como gestor de frota, eu quero receber alertas automáticos sobre manutenções próximas, para que eu possa agendar manutenções preventivas antes que se tornem urgentes.

#### Acceptance Criteria

1. THE FleetManager SHALL execute the MaintenanceCheckJob daily at midnight
2. WHEN MaintenanceCheckJob executes, THE FleetManager SHALL identify all Vehicles with NextMaintenanceDate within 3 days
3. WHEN a Vehicle has upcoming maintenance, THE FleetManager SHALL log an alert message with the Vehicle Plate and NextMaintenanceDate
4. THE FleetManager SHALL use Quartz.NET to schedule and execute the MaintenanceCheckJob
5. THE FleetManager SHALL log all MaintenanceCheckJob executions using Serilog

### Requirement 6

**User Story:** Como gestor de frota, eu quero registrar viagens realizadas, para que eu possa rastrear o uso dos veículos e condutores.

#### Acceptance Criteria

1. THE FleetManager SHALL provide REST endpoints to start and end Trip records
2. WHEN a Trip is started, THE FleetManager SHALL validate that the VehicleId and DriverId are valid and available
3. WHEN a Trip is started, THE FleetManager SHALL update the Vehicle Status to InUse
4. WHEN a Trip is ended, THE FleetManager SHALL calculate the Distance based on StartDate and EndDate
5. WHEN a Trip is ended, THE FleetManager SHALL update the Vehicle Status to Available and increment the Vehicle Mileage

### Requirement 7

**User Story:** Como gestor de frota, eu quero consultar a disponibilidade de veículos e condutores, para que eu possa planejar viagens eficientemente.

#### Acceptance Criteria

1. THE FleetManager SHALL provide an endpoint to retrieve all Vehicles with Status equal to Available
2. THE FleetManager SHALL provide an endpoint to retrieve all Drivers with Active equal to true
3. WHEN a Vehicle is InMaintenance or InUse, THE FleetManager SHALL exclude it from the available Vehicles list
4. THE FleetManager SHALL return Vehicle availability status in real-time based on current Trip and maintenance records

### Requirement 8

**User Story:** Como gestor de frota, eu quero que o sistema atualize automaticamente o status dos veículos, para que eu tenha informações precisas sobre a disponibilidade da frota.

#### Acceptance Criteria

1. WHEN a Trip is started, THE FleetManager SHALL change the Vehicle Status from Available to InUse
2. WHEN a Trip is ended, THE FleetManager SHALL change the Vehicle Status from InUse to Available
3. WHEN a MaintenanceRecord is created, THE FleetManager SHALL change the Vehicle Status to InMaintenance
4. WHEN maintenance is completed, THE FleetManager SHALL allow manual update of Vehicle Status to Available
5. THE FleetManager SHALL validate Status transitions to prevent invalid state changes

### Requirement 9

**User Story:** Como desenvolvedor, eu quero que o sistema siga Clean Architecture, para que o código seja mantível, testável e escalável.

#### Acceptance Criteria

1. THE FleetManager SHALL organize code into Domain, Application, Infrastructure, and Api layers
2. THE FleetManager SHALL implement the Repository pattern in the Infrastructure layer for data access
3. THE FleetManager SHALL define use cases in the Application layer independent of infrastructure concerns
4. THE FleetManager SHALL place domain entities and business logic in the Domain layer
5. THE FleetManager SHALL ensure that the Domain layer has no dependencies on external frameworks

### Requirement 10

**User Story:** Como desenvolvedor, eu quero que o sistema tenha testes automatizados, para que eu possa garantir a qualidade e confiabilidade do código.

#### Acceptance Criteria

1. THE FleetManager SHALL include unit tests for domain logic using xUnit
2. THE FleetManager SHALL include integration tests for API endpoints using xUnit
3. THE FleetManager SHALL use Moq for mocking dependencies in unit tests
4. THE FleetManager SHALL achieve test coverage for critical business logic including maintenance calculations and status transitions
5. THE FleetManager SHALL include tests for the MaintenanceCheckJob execution
