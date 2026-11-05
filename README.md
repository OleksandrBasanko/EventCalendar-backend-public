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
```

---
 
## Getting Started with Docker
 
This section describes how to download and run the EventCalendar project locally using Docker.
 
### Required Tools
 
- WSL (Windows Subsystem for Linux) with Ubuntu installed
- Docker (available inside WSL)
- pgAdmin 4 (to create the PostgreSQL server)
### 1. Create the PostgreSQL Server
 
On your machine, create a PostgreSQL server (you can use pgAdmin 4 for this) with the following parameters:
 
| Tab | Parameter | Value |
|---|---|---|
| General | Name | `Event-Calendar` |
| Connection | Host name / address | `localhost` |
| Connection | Port | `5002` |
| Connection | Username | `admin` |
| Connection | Password | `adminpass` |
 
### 2. Prepare the Project Folders

Link to the frontend repository: <https://github.com/OleksandrBasanko/EventCalendar-frontend-public>
 
1. Create an empty folder on your disk and name it `EventCalendar`.
2. Inside this folder, create two more empty folders: `event-calendar-frontend` and `event-calendar-backend`.
3. Download the files from each repository into the corresponding folder:
   - `EventCalendar-backend-public` → `event-calendar-backend`
   - `EventCalendar-frontend-public` → `event-calendar-frontend`
4. Move the `docker-compose.yaml` file from the `event-calendar-backend` folder into the project root folder (`EventCalendar`).
The resulting structure should look like this:
 
```
EventCalendar/
├── docker-compose.yaml
├── event-calendar-backend/
└── event-calendar-frontend/
```
 
### 3. Build the Docker Images
 
Open the WSL console and navigate to the project root folder:
 
```bash
cd /mnt/{full path to the folder}
```
 
Example:
 
```bash
cd /mnt/d/EventCalendar/
```
 
Go into the `event-calendar-backend` folder and build the backend image:
 
```bash
cd event-calendar-backend
sudo docker build -t event-calendar-backend .
```
 
Go back to the root folder, then go into the `event-calendar-frontend` folder and build the frontend image:
 
```bash
cd ..
cd event-calendar-frontend
sudo docker build -t event-calendar-frontend .
```
 
### 4. Run the Project
 
Return to the project root folder and start the containers:
 
```bash
cd ..
sudo docker compose up -d
```
 
If everything was done correctly, you should see the following output:
 
```
 ✔ Container event_calendar_postgres Running
 ✔ Container event_calendar_api      Running
 ✔ Container event_calendar_ui       Running
```
 
You can now view the project in your browser at: <http://localhost:5005/>
 
---
 
## Seeding Test Data
 
To populate the website with test data, first **register** an account on the site, then run the following commands in order.
 
> **Note:** In the commands below, replace `your@email.com` with the email address you used during registration.
 
### 1. Make Your User an Admin
 
```bash
sudo docker compose exec -T postgres psql -U admin -d Event_Calendar -v email="your@email.com" < event-calendar-backend/scripts/make_admin.sql
```
 
### 2. Create Test Users
 
```bash
bash event-calendar-backend/scripts/seed_users.sh
```
 
### 3. Create Events
 
```bash
sudo docker compose exec -T postgres psql -U admin -d Event_Calendar -v email="your@email.com" < event-calendar-backend/scripts/seed_events.sql
```
 
### 4. Subscribe Test Users to Events
 
```bash
sudo docker compose exec -T postgres psql -U admin -d Event_Calendar < event-calendar-backend/scripts/subscribe_users.sql
```
 
That's it — everything is ready!
 
### Testing as a Regular User
 
To test the website as a regular user rather than an admin, you can log in as one of the previously created test users. Go to <http://localhost:5005/login> and use one of the following accounts:
 
| Email | Password |
|---|---|
| `anna@test.com` | `Test123!` |
| `ivan@test.com` | `Test123!` |
| `olena@test.com` | `Test123!` |
