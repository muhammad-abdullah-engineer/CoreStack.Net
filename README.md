# 🚀 CoreStack.NET - Enterprise-Grade ASP.NET Core Web API

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](CONTRIBUTING.md)
[![Test Coverage](https://img.shields.io/badge/Coverage-Comprehensive-blue.svg)](#-code-quality-metrics)

> **CoreStack.NET** is a **production-ready**, **scalable**, and **secure** RESTful API built with **Clean Architecture**, **ASP.NET Core 10.0**, and **Entity Framework Core**. Designed as both a **Starter Template** and **Learning Resource**, it demonstrates enterprise-grade patterns and best practices for building robust .NET applications.
>
> Perfect for:
> - 💼 **Recruiters** evaluating backend development expertise
> - 👨‍💻 **Developers** learning Clean Architecture and modern ASP.NET Core
> - 🎓 **Students** studying enterprise software design patterns
> - 🏢 **Teams** building production-ready APIs with proven patterns

## 📋 Table of Contents

- [Overview](#-overview)
- [Why CoreStack.NET?](#-why-coresStacknet)
- [Key Features](#-key-features)
- [Code Quality Metrics](#-code-quality-metrics)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Getting Started](#-getting-started)
- [API Documentation](#-api-documentation)
- [Database](#-database)
- [Authentication & Security](#-authentication--security)
- [Scalability & Performance](#-scalability--performance)
- [Background Jobs](#-background-jobs)
- [Testing](#-testing)
- [Project Structure](#-project-structure)
- [Configuration](#-configuration)
- [For Recruiters](#-for-recruiters)
- [Contributing](#-contributing)
- [License](#-license)

## 🎯 Overview

**CoreStack.NET** is a comprehensive, enterprise-level RESTful web service crafted to showcase **best practices** and **production-ready patterns** in modern .NET development. Built with **ASP.NET Core 10.0**, it demonstrates professional software engineering principles including:

- ✅ **Clean Architecture** - Strict separation of concerns with Domain, Application, Infrastructure, and API layers
- ✅ **SOLID Principles** - Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- ✅ **Dependency Injection** - Proper service registration and lifetime management
- ✅ **Test-Driven Development (TDD)** - Comprehensive test coverage with xUnit
- ✅ **Industry Best Practices** - Security, validation, error handling, and logging
- ✅ **Scalable Architecture** - Ready for microservices migration and high-load scenarios

This repository serves as both a **starter template** for new projects and a **learning resource** for understanding enterprise .NET patterns.

## 🌟 Why CoreStack.NET?

### For Developers & Learners
- **Learn by Example**: Real-world implementation of Clean Architecture principles, not just theory
- **Production-Ready Code**: Copy patterns directly into your projects with confidence
- **Well-Documented**: Every architectural decision is explained with reasoning
- **Modern C# Features**: Uses C# 12.0 features like primary constructors and pattern matching
- **Extensible Design**: Easily adapt and extend for your specific needs

### For Recruiters & Hiring Managers
- **Showcase Quality**: Demonstrates understanding of enterprise architecture and design patterns
- **Code Quality**: Comprehensive test coverage, proper error handling, and validation
- **Best Practices**: Shows commitment to SOLID principles, Clean Code, and industry standards
- **Scalability Awareness**: Architected for growth without major refactoring
- **Professional Approach**: Version control, documentation, and contribution guidelines

### Why Choose CoreStack.NET Over Alternatives?
- ✨ **Comprehensive**: Covers full stack from authentication to background jobs
- 🎯 **Focused**: Task Management domain is simple enough to understand but complex enough to be realistic
- 📚 **Educational**: Includes explanations and rationale, not just code
- 🔒 **Secure**: Implements JWT, BCrypt, input validation, and security best practices
- ⚡ **Modern Stack**: ASP.NET Core 9.0, PostgreSQL, Entity Framework Core 9.0
- 🧪 **Well-Tested**: Comprehensive unit and integration tests with xUnit

## ✨ Key Features

### Core Functionality
- ✅ **RESTful API Design** - Industry-standard REST principles with proper HTTP methods and status codes
- 🔐 **JWT Authentication** - Secure token-based authentication with BCrypt password hashing
- 📝 **CRUD Operations** - Complete Create, Read, Update, Delete operations for task management
- 🎯 **Task Priorities & Status** - Built-in support for task priorities (Low, Medium, High, Critical) and statuses
- 👥 **User Management** - Full user registration, login, and authentication system
- ⏰ **Background Jobs** - Automated task scheduling with Hangfire for cron jobs and queued tasks
- 📊 **Hangfire Dashboard** - Real-time job monitoring and management UI

### Technical Excellence
- 🏗️ **Clean Architecture** - Separation of concerns with Domain, Application, Infrastructure, and API layers
- 🧪 **Unit Testing** - Comprehensive test coverage using xUnit framework
- ✔️ **Input Validation** - FluentValidation for robust request validation
- 🗺️ **AutoMapper** - Efficient object-to-object mapping between entities and DTOs
- 🔄 **Repository Pattern** - Generic repository implementation for data access abstraction
- 🛡️ **Global Exception Handling** - Custom middleware for centralized error handling
- 📝 **Request Logging** - Middleware for logging all HTTP requests and responses
- 📖 **Swagger/OpenAPI** - Interactive API documentation with built-in testing capabilities
- 🌐 **CORS Support** - Configured for cross-origin resource sharing

## 📊 Code Quality Metrics

### Test Coverage & Quality
- **Unit Test Coverage**: Comprehensive tests for Services, Validators, and Repositories
- **Integration Tests**: End-to-end API testing with `Microsoft.AspNetCore.Mvc.Testing`
- **Test Framework**: xUnit with Moq for dependency mocking
- **Code Standards**: EditorConfig ensures consistent coding style across the team
- **Best Practices**: Follows industry-standard patterns and SOLID principles

### Key Quality Indicators
| Metric | Status |
|--------|--------|
| **Architecture Pattern** | Clean Architecture ✅ |
| **Design Principles** | SOLID Compliant ✅ |
| **Exception Handling** | Global Middleware ✅ |
| **Input Validation** | FluentValidation ✅ |
| **Authentication** | JWT + BCrypt ✅ |
| **API Documentation** | Swagger/OpenAPI ✅ |
| **Data Persistence** | EF Core Migrations ✅ |
| **Dependency Injection** | Configured & Tested ✅ |
| **Request Logging** | Middleware Implemented ✅ |
| **CORS Support** | AllowAll Policy ✅ |

### Technical Decision Rationale

**Why Clean Architecture?**
- Enforces separation of concerns making code maintainable and testable
- Each layer has a single responsibility
- Easy to swap implementations (e.g., switching databases)
- Reduces dependencies and coupling

**Why Entity Framework Core?**
- LINQ integration for type-safe queries
- Built-in migration system for version control
- Automatic change tracking
- Industry standard for .NET ORM

**Why JWT with BCrypt?**
- **JWT**: Stateless authentication, perfect for APIs and microservices
- **BCrypt**: Computationally expensive hashing prevents brute-force attacks
- No session state needed, scales horizontally

**Why Hangfire?**
- Reliable background job processing
- SQL Server storage ensures job persistence
- Dashboard for monitoring and debugging
- Cron job support for scheduled tasks

**Why SQL Server?**
- Enterprise-grade, highly reliable, excellent for scalability
- ACID compliance for data integrity
- Deep integration with .NET ecosystem
- Powerful query optimization and management tools
- LocalDB available for development without separate installation

## 🛠 Tech Stack

### Backend Framework
- **ASP.NET Core 10.0** - Latest .NET framework for building modern web APIs
- **C# 12.0** - Latest C# language features including primary constructors and pattern matching
- **Entity Framework Core 10.0** - Modern ORM for database operations

### Database & Storage
- **SQL Server** - Relational database for data persistence
- **Entity Framework Core Migrations** - Database version control and schema management

### Authentication & Security
- **JWT Bearer Tokens** - Industry-standard authentication mechanism
- **BCrypt** - Secure password hashing algorithm
- **ASP.NET Core Identity** - User authentication and authorization framework

### Background Processing
- **Hangfire** - Reliable background job processing with job persistence
- **Hangfire Dashboard** - Web-based UI for monitoring jobs

### Validation & Mapping
- **FluentValidation** - Fluent interface for building validation rules
- **AutoMapper** - Convention-based object-to-object mapper

### Documentation & Testing
- **Swashbuckle (Swagger)** - OpenAPI specification and UI
- **xUnit** - Modern testing framework for .NET
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing for ASP.NET Core

### Code Quality
- **SonarQube Integration** - Static code analysis and quality metrics
- **EditorConfig** - Consistent coding styles across the team

## 🏛 Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                     API Layer (Presentation)             │
│  Controllers, Middleware, Program.cs, Dependency Setup  │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│              Application Layer (Business Logic)          │
│  Services, Interfaces, ViewModels, Validators, Mappings │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│         Infrastructure Layer (Data & External)           │
│   EF Core, Repositories, JWT, Password Hashing, Jobs    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│               Domain Layer (Core Business)               │
│         Entities, Enums, Domain Models (No deps)        │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

#### 🎨 **Domain Layer** (`TaskManagement.Domain`)
- Core business entities (`User`, `TaskItem`)
- Enums (`TaskStatus`, `TaskPriority`)
- Base entities with common properties
- **Zero external dependencies** - Pure business logic

#### 📊 **Application Layer** (`TaskManagement.Application`)
- Service interfaces and implementations (`IAuthService`, `ITaskService`)
- ViewModels for requests and responses
- FluentValidation validators
- AutoMapper mapping profiles
- Business logic and use cases
- **Depends only on Domain layer**

#### 🔌 **Infrastructure Layer** (`TaskManagement.Infrastructure`)
- Entity Framework Core `DbContext` and migrations
- Repository implementations
- JWT token generation service
- BCrypt password hashing service
- Hangfire background job implementations
- **Depends on Application and Domain layers**

#### 🌐 **API Layer** (`TaskManagement.API`)
- ASP.NET Core Web API controllers
- Middleware (exception handling, request logging)
- Dependency injection configuration
- Startup configuration
- **Depends on all other layers**

## ⚡ Scalability & Performance

### Architecture for Scale
- **Stateless API Design**: Each request is independent, enables horizontal scaling
- **Repository Pattern**: Easy to implement caching layers (Redis, Memcached)
- **Async/Await Throughout**: Non-blocking operations for better throughput
- **Dependency Injection**: Enables swapping implementations without code changes
- **Background Jobs with Hangfire**: Offload long-running operations

### Performance Considerations
- **Entity Framework Core**: Lazy loading prevention, query optimization
- **Connection Pooling**: PostgreSQL connection reuse
- **Request Logging Middleware**: Minimal overhead, can be disabled in production
- **JWT Tokens**: Stateless authentication reduces database queries
- **AutoMapper**: Efficient object mapping with configuration caching

### Scalability Roadmap
- **Horizontal Scaling**: Stateless API can run on multiple servers with load balancer
- **Caching Layer**: Redis integration for frequently accessed data
- **Database Read Replicas**: PostgreSQL supports read replicas for analytics
- **Microservices**: Clean Architecture makes it easy to split into microservices
- **API Rate Limiting**: Can be added via middleware or API Gateway
- **GraphQL Support**: Can coexist with REST endpoints

### Benchmarking Results (Example Scenarios)
| Scenario | Performance |
|----------|-------------|
| **Login Request** | ~50ms (with BCrypt hashing) |
| **List Tasks (10K)** | ~200ms (EF Core query optimization) |
| **Create Task + Background Job** | ~100ms (async operation) |
| **Concurrent Users (100)** | Sustained with 0% errors |
| **Database Connections** | Connection pooling reduces overhead |

**Note**: These are example benchmarks. Run your own tests with `dotnet run --project src/TaskManagement.API/TaskManagement.API.csproj` and use tools like Apache JMeter or Postman for load testing.

## 🚀 Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or later
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) or SQL Server LocalDB
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) (optional but recommended)
- [Git](https://git-scm.com/) for version control

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/muhammad-abdullah-engineer/CoreStack.NET.git
   cd CoreStack.NET
   ```

2. **Configure the database connection**

   Update the connection string in `src/TaskManagement.API/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
   }
   ```

   **For Production**: Update with your SQL Server instance details and use secure credentials from environment variables or secrets management.

3. **Update JWT settings** (optional, defaults are provided)
   ```json
   {
     "Jwt": {
       "Key": "your-secret-key-minimum-32-characters-long",
       "Issuer": "TaskManagementAPI",
       "Audience": "TaskManagementClient",
       "ExpiryInHours": 24
     }
   }
   ```
   **Security Note**: Generate a strong secret key with at least 32 characters. Use `dotnet user-secrets` for development.

4. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

5. **Build the solution**
   ```bash
   dotnet build
   ```

6. **Apply database migrations** (automatic on first run, or manually)
   ```bash
   cd src/TaskManagement.API
   dotnet ef database update
   ```

7. **Run the application**
   ```bash
   dotnet run --project src/TaskManagement.API/TaskManagement.API.csproj
   ```

8. **Access the API**
   - Swagger UI: `https://localhost:63496/` or `http://localhost:63497/`
   - Hangfire Dashboard: `https://localhost:63496/hangfire`

### Quick Start with Docker (Coming Soon)

```bash
docker-compose up -d
```

## 📖 API Documentation

### Interactive Documentation

Once the application is running, visit the **Swagger UI** at the root URL for interactive API documentation:
- **HTTPS**: `https://localhost:63496/`
- **HTTP**: `http://localhost:63497/`

### Authentication Endpoints

#### Register a New User
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecureP@ssw0rd"
}
```

**Response:**
```json
{
  "success": true,
  "message": "User registered successfully",
  "data": {
    "userId": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
  }
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecureP@ssw0rd"
}
```

### Task Management Endpoints

#### Get All Tasks (Requires Authentication)
```http
GET /api/tasks
Authorization: Bearer {your-jwt-token}
```

#### Create a Task
```http
POST /api/tasks
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
  "title": "Complete project documentation",
  "description": "Write comprehensive README and API docs",
  "priority": "High",
  "status": "InProgress",
  "dueDate": "2024-12-31T23:59:59Z"
}
```

#### Update a Task
```http
PUT /api/tasks/{id}
Authorization: Bearer {your-jwt-token}
Content-Type: application/json

{
  "title": "Updated task title",
  "description": "Updated description",
  "priority": "Critical",
  "status": "Completed"
}
```

#### Delete a Task
```http
DELETE /api/tasks/{id}
Authorization: Bearer {your-jwt-token}
```

### Task Status Values
- `Todo` - Task is pending
- `InProgress` - Task is being worked on
- `Completed` - Task is finished
- `Cancelled` - Task was cancelled

### Task Priority Values
- `Low` - Low priority
- `Medium` - Medium priority
- `High` - High priority
- `Critical` - Urgent/critical priority

## 🗄 Database

### Database Schema

The application uses **SQL Server** with Entity Framework Core for database operations.

#### Tables

**Users**
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| Username | nvarchar(100) | Unique username |
| Email | nvarchar(255) | Unique email address |
| PasswordHash | nvarchar(max) | BCrypt hashed password |
| CreatedAt | datetime2 | Auto-generated creation timestamp |
| UpdatedAt | datetime2 | Auto-updated timestamp |

**Tasks**
| Column | Type | Description |
|--------|------|-------------|
| Id | int | Primary key |
| Title | nvarchar(200) | Task title |
| Description | nvarchar(max) | Task description |
| Status | int | Task status enum |
| Priority | int | Task priority enum |
| DueDate | datetime2 | Optional due date |
| UserId | int | Foreign key to Users |
| CreatedAt | datetime2 | Auto-generated creation timestamp |
| UpdatedAt | datetime2 | Auto-updated timestamp |

### Migrations

Entity Framework Core migrations are automatically applied on application startup. To create new migrations:

```bash
cd src/TaskManagement.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../TaskManagement.API/TaskManagement.API.csproj
dotnet ef database update --startup-project ../TaskManagement.API/TaskManagement.API.csproj
```

## 🔐 Authentication & Security

### JWT (JSON Web Tokens)

This API uses **JWT Bearer tokens** for stateless authentication:

1. User registers or logs in
2. Server generates and returns a JWT token
3. Client includes token in `Authorization` header for protected endpoints
4. Server validates token on each request

**Token Structure:**
```json
{
  "sub": "user-id",
  "email": "user@example.com",
  "jti": "unique-token-id",
  "exp": 1735689600
}
```

### Password Security

- Passwords are hashed using **BCrypt** with auto-generated salt
- Original passwords are never stored in the database
- BCrypt is computationally expensive, providing protection against brute-force attacks

### Security Best Practices Implemented

- ✅ Password hashing with BCrypt
- ✅ JWT token expiration
- ✅ HTTPS support
- ✅ CORS configuration
- ✅ Input validation with FluentValidation
- ✅ SQL injection protection via EF Core parameterization
- ✅ Global exception handling (no sensitive data exposure)

## ⏰ Background Jobs

### Hangfire Integration

The API includes **Hangfire** for reliable background job processing with:

- 🔄 **Recurring Jobs (Cron)** - Scheduled tasks that run at specific intervals
- 📬 **Queue Jobs** - On-demand background jobs triggered by API calls
- 📊 **Dashboard** - Web-based UI for monitoring jobs at `/hangfire`

### Example Background Jobs

#### Cron Jobs (Scheduled)
- **Cleanup Old Tasks** - Daily at 2:00 AM - Removes completed tasks older than 30 days
- **Process Task Reminders** - Daily at 9:00 AM - Sends reminders for tasks due today
- **Generate Daily Report** - Mon-Fri at 5:00 PM - Creates daily task summary reports

#### Queue Jobs (On-Demand)
- **Send Email Notification** - Triggered when important task events occur
- **Export Tasks to CSV** - Generate CSV export of user tasks
- **Send Task Notification** - Push notifications for task updates

### Accessing Hangfire Dashboard

Navigate to `https://localhost:63496/hangfire` to view:
- Job execution history
- Scheduled jobs
- Failed jobs with retry information
- Server statistics

## 🧪 Testing

### Running Tests

Execute all tests:
```bash
dotnet test
```

Run specific test:
```bash
dotnet test --filter "FullyQualifiedName~TestName"
```

Run tests with coverage:
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Test Structure

```
tests/
└── TaskManagement.Tests/
    ├── Controllers/
    │   ├── AuthControllerTests.cs
    │   └── TasksControllerTests.cs
    ├── Services/
    │   ├── AuthServiceTests.cs
    │   └── TaskServiceTests.cs
    └── Validators/
        └── ViewModelValidatorTests.cs
```

### Testing Framework

- **xUnit** - Primary testing framework
- **Moq** - Mocking framework for dependencies
- **FluentAssertions** - Assertion library
- **Microsoft.AspNetCore.Mvc.Testing** - Integration testing

## 📁 Project Structure

```
TaskManagementAPI/
├── src/
│   ├── TaskManagement.Domain/              # Core domain models
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── User.cs
│   │   │   └── TaskItem.cs
│   │   └── Enums/
│   │       ├── TaskStatus.cs
│   │       └── TaskPriority.cs
│   │
│   ├── TaskManagement.Application/         # Business logic layer
│   │   ├── Interfaces/
│   │   │   ├── IAuthService.cs
│   │   │   ├── ITaskService.cs
│   │   │   └── IRepository.cs
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   └── TaskService.cs
│   │   ├── ViewModels/
│   │   │   ├── Auth/
│   │   │   │   ├── RegisterViewModel.cs
│   │   │   │   ├── LoginViewModel.cs
│   │   │   │   └── AuthResponseViewModel.cs
│   │   │   ├── Tasks/
│   │   │   │   ├── CreateTaskViewModel.cs
│   │   │   │   ├── UpdateTaskViewModel.cs
│   │   │   │   └── TaskResponseViewModel.cs
│   │   │   └── Common/
│   │   │       └── ApiResponse.cs
│   │   ├── Validators/
│   │   │   └── Auth/
│   │   │       ├── RegisterViewModelValidator.cs
│   │   │       └── LoginViewModelValidator.cs
│   │   └── Mappings/
│   │       └── MappingProfile.cs
│   │
│   ├── TaskManagement.Infrastructure/      # Data access & external services
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Migrations/
│   │   ├── Repositories/
│   │   │   └── Repository.cs
│   │   ├── Services/
│   │   │   ├── JwtTokenService.cs
│   │   │   └── PasswordHasher.cs
│   │   └── BackgroundJobs/
│   │       ├── Configurations/
│   │       ├── Implementations/
│   │       └── Jobs/
│   │
│   └── TaskManagement.API/                 # Web API entry point
│       ├── Controllers/
│       │   ├── AuthController.cs
│       │   └── TasksController.cs
│       ├── Middlewares/
│       │   ├── ExceptionHandlingMiddleware.cs
│       │   └── RequestLoggingMiddleware.cs
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       └── Program.cs
│
├── tests/
│   └── TaskManagement.Tests/              # Unit & integration tests
│       ├── Controllers/
│       ├── Services/
│       └── Validators/
│
├── .editorconfig                           # Code style configuration
├── .gitignore
├── TaskManagementAPI.sln                   # Solution file
├── README.md
└── LICENSE
```

## ⚙ Configuration

### Application Settings

Configuration is managed through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TaskManagementDB;Trusted_Connection=True"
  },
  "Jwt": {
    "Key": "your-secret-key-at-least-32-characters-long",
    "Issuer": "TaskManagementAPI",
    "Audience": "TaskManagementClient",
    "ExpiryInHours": 24
  },
  "Hangfire": {
    "ServerName": "TaskManagementAPI",
    "WorkerCount": 4,
    "DashboardPath": "/hangfire"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Hangfire": "Information"
    }
  }
}
```

### Environment-Specific Configuration

- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- `appsettings.Production.json` - Production overrides (not in repo)

### User Secrets (Development)

For sensitive data in development, use .NET User Secrets:

```bash
cd src/TaskManagement.API
dotnet user-secrets set "Jwt:Key" "your-secret-key"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

## 👔 For Recruiters

### What This Project Demonstrates

CoreStack.NET is designed to showcase professional backend development skills to technical hiring managers and recruiters:

#### Architecture & Design
- ✅ **Clean Architecture Implementation** - Clear separation of concerns across 4 layers
- ✅ **SOLID Principles** - Every principle correctly applied and documented
- ✅ **Design Patterns** - Repository Pattern, Dependency Injection, Factory Pattern
- ✅ **Scalability Thinking** - Stateless design ready for horizontal scaling

#### Code Quality
- ✅ **Production-Ready Code** - Professional error handling, validation, and logging
- ✅ **Security Awareness** - JWT authentication, BCrypt hashing, input validation
- ✅ **Test-Driven Development** - Comprehensive unit and integration tests
- ✅ **Documentation** - Well-documented code and architecture decisions

#### Technical Competencies Demonstrated
- ✅ Backend API Development (.NET Core, C#)
- ✅ Database Design & ORM (Entity Framework Core, PostgreSQL)
- ✅ Authentication & Security (JWT, BCrypt)
- ✅ API Documentation (Swagger/OpenAPI)
- ✅ Software Architecture (Clean Architecture, SOLID)
- ✅ Testing (xUnit, Integration Tests)
- ✅ Background Job Processing (Hangfire)
- ✅ Asynchronous Programming (async/await)
- ✅ Git & Version Control

#### For Hiring Managers
This repository demonstrates:
- **Professional Communication**: Clear README, well-documented code
- **Scalability Mindset**: Designed for growth and microservices migration
- **Best Practices Adherence**: Industry-standard patterns and security practices
- **Problem-Solving**: Thoughtful architectural decisions with documented rationale
- **Attention to Detail**: Comprehensive testing, error handling, validation
- **Team Readiness**: Code style standards, contribution guidelines, clean commits

### How to Review This Project
1. Start with this README to understand the architecture
2. Review `src/TaskManagement.API/Program.cs` for dependency injection setup
3. Examine `src/TaskManagement.Application/Services/` for business logic
4. Check `tests/TaskManagement.Tests/` for testing approach
5. Review commit history for clean, professional commits

### Interview Questions for Candidates Using CoreStack.NET
- "Why did you choose Clean Architecture for this project?"
- "How would you handle database scaling in this application?"
- "Explain your approach to error handling and logging"
- "What security measures did you implement and why?"
- "How would you add caching to improve performance?"
- "Can you describe the flow from a user registration request to database storage?"

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. **Fork the repository**
2. **Create a feature branch**
   ```bash
   git checkout -b feature/amazing-feature
   ```
3. **Commit your changes**
   ```bash
   git commit -m 'Add some amazing feature'
   ```
4. **Push to the branch**
   ```bash
   git push origin feature/amazing-feature
   ```
5. **Open a Pull Request**

### Coding Standards

- Follow C# naming conventions
- Write XML documentation for public APIs
- Include unit tests for new features
- Update README for significant changes
- Follow SOLID principles

### Commit Message Guidelines

- Use present tense ("Add feature" not "Added feature")
- Use imperative mood ("Move cursor to..." not "Moves cursor to...")
- Limit first line to 72 characters
- Reference issues and pull requests

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Muhammad Abdullah**
- GitHub: [@muhammad-abdullah-engineer](https://github.com/muhammad-abdullah-engineer)
- Portfolio: [CoreStack.NET Repository](https://github.com/muhammad-abdullah-engineer/CoreStack.NET)
- Focus: Enterprise-grade .NET architecture, Clean Code, and best practices

## 🌟 Acknowledgments

- Built with [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- Clean Architecture principles by [Robert C. Martin](https://blog.cleancoder.com/)
- Inspired by modern microservices architecture

## 📊 Project Status

🟢 **Active Development** - This project is actively maintained and open for contributions.

### Roadmap

**Short Term (v1.0 - v1.5)**
- [ ] Docker containerization & docker-compose
- [ ] CI/CD pipeline with GitHub Actions
- [ ] Automated deployment to Azure/AWS
- [ ] Code coverage badges in README
- [ ] Performance benchmarking suite

**Medium Term (v2.0 - v2.5)**
- [ ] Redis caching layer integration
- [ ] API rate limiting middleware
- [ ] Advanced search and filtering
- [ ] Bulk operations API endpoints
- [ ] Comprehensive API documentation with examples

**Long Term (v3.0+)**
- [ ] GraphQL API support (alongside REST)
- [ ] Real-time notifications (SignalR)
- [ ] Elasticsearch integration
- [ ] Event-driven architecture
- [ ] OAuth2/OpenID Connect integration
- [ ] Multi-tenancy support
- [ ] Microservices decomposition

---

<div align="center">

**CoreStack.NET** - Building enterprise-grade applications with .NET 🚀

If this project helped you learn or inspired your architecture, please give it a ⭐ **star**!

Made with ❤️ by [Muhammad Abdullah](https://github.com/muhammad-abdullah-engineer) using ASP.NET Core

---

**Ready to use CoreStack.NET as a starter template for your next project?**
Fork it, clone it, and build something amazing! 🎯

</div>
