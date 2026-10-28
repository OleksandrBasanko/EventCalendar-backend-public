# EventsCalendar

EventsCalendar is an event calendar web service. Any site visitor can browse the calendar and see which dates have scheduled events (e.g., concerts). Dates with events are visually highlighted among others. Clicking on such a date displays a list of scheduled events, and if registered, the user can subscribe to an event (indicated by a checkmark next to the event).

## Features

- **Calendar Browsing** — Available to all visitors without registration. Dates with scheduled events are visually highlighted among regular (grey) dates.
- **View Events for a Specific Date** — Clicking on a highlighted date opens a list of events scheduled for that day.
- **Event Subscription** — Available only to registered users. After subscribing, a checkmark appears next to the event.
- **User Roles** — Global (`User`, `Admin`) and event-specific roles within a particular event (`EventOwner`, `EventAdmin`, `EventModerator`, `EventSubscriber`).

## Tech Stack

- **.NET 10.0**
- **ASP.NET Core Web API**
- **Clean Architecture** using SOLID principles (separation into API, Infrastructure, Application, and Domain layers)
- **Entity Framework Core 10.0.9** (Code First approach)
- **PostgreSQL** — database
- **Npgsql.EntityFrameworkCore.PostgreSQL 10.0.2** — EF Core provider for PostgreSQL
- **MediatR 14.2.0** — implementation of the CQRS approach on the Application layer
- **Microsoft.AspNetCore.Authentication.JwtBearer 10.0.9** — JWT authentication
- **Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.9** — user and role management (Identity)
- **Swashbuckle.AspNetCore 10.2.3** and **Microsoft.AspNetCore.OpenApi 10.0.7** — API documentation and generation (Swagger / OpenAPI)
- **Microsoft.EntityFrameworkCore.Design / Tools 10.0.9** — database migration management tools

## Database Connection
The PostgreSQL database connection was established using pgAdmin 4

## Project Structure

Clean Architecture

```
EventsCalendar/
├── EventsCalendar.API/              → Application, Domain, Infrastructure
├── EventsCalendar.Infrastructure/   → Application, Domain
├── EventsCalendar.Application/      → Domain
└── EventsCalendar.Domain/           (independent of anything)
```

| Project | Purpose |
|---|---|
| **API** | Entry point, controllers (`[ApiController]`, authorization `[Authorize]`, Swagger), DI service registration, and configuration. Uses `MediatR` to dispatch requests/commands to business logic, extracts input models into `Requests`, and handles errors via a centralized `ExceptionMiddleware`. |
| **Infrastructure** | Data access implementation: `AppDbContext`, isolated entity configurations (`IEntityTypeConfiguration`) for code cleanliness, repositories, and EF Core migrations. User and security management via ASP.NET Core Identity (`ApplicationUser`, `IdentityService`). Registration of all layer dependencies is performed via a centralized DI extension method (`AddInfrastructure`). |
| **Application** | System business logic implemented using the **CQRS approach** (separating operations into **Commands** for state changes and **Queries** for data reading via MediatR). Contains abstraction interfaces (`IUnitOfWork`, repositories) and **DTOs** returned instead of domain entities to prevent circular dependencies during serialization. The layer does not depend directly on EF Core or the database. |
| **Domain** | The core of the system without external dependencies. Contains entities (`Event`, `UserEvent`) built using the **Rich Domain Model** pattern (with state encapsulation, validation in constructors/methods, and factory methods), as well as custom exceptions and role enums. |

## Database Schema

**Users** — registered users (managed via ASP.NET Core Identity)
| Field | Type |
|---|---|
| Id | Guid (PK) |
| FirstName, SecondName, UserName | text |
| Email, PhoneNumber | text |
| CreatedAt, UpdatedAt | timestamp |

**Events** — calendar events
| Field | Type |
|---|---|
| Id | Guid (PK) |
| EventName | text |
| ShortDescription, DetailedDescription | text |
| StartDate, EndDate | timestamp |
| CreateAt | timestamp |
| OwnerId | Guid (FK → Users.Id) |

**UserEvents** — user subscriptions to events (many-to-many relationship)
| Field | Type |
|---|---|
| UserId | Guid (FK → Users.Id, composite PK) |
| EventId | Guid (FK → Events.Id, composite PK) |
| EventRole | integer (enum: EventOwner = 0, EventSubscriber = 1) |

Relationships:
- `Events.OwnerId → Users.Id` (Restrict on delete)
- `UserEvents.UserId → Users.Id` (Cascade on delete)
- `UserEvents.EventId → Events.Id` (Cascade on delete)


### Prerequisites

- .NET 10.0 SDK
- PostgreSQL server (remote)
- `dotnet-ef` CLI tool:
  ```bash
  dotnet tool install --global dotnet-ef
