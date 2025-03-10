# E-Learning Platform API

A GraphQL-based Web API for an e-learning platform built with .NET Core, following Clean Architecture and Domain-Driven Design principles.

## Project Overview

This project implements a modern e-learning platform where instructors can create courses with modules, lessons, and assignments, while students can enroll in courses, track their progress, and submit assignments for evaluation.

## Technology Stack

- **.NET Core**: Backend framework
- **GraphQL**: API query language for flexible data retrieval
- **REST API**: Traditional endpoints for specific operations
- **Dapr**: Distributed Application Runtime for read operations
- **Entity Framework Core**: ORM for create, update, and delete operations
- **Clean Architecture**: Architectural pattern
- **Domain-Driven Design**: Design approach

## Project Structure

The solution follows Clean Architecture with these layers:

```
ELearning/
├── Domain/              # Core business logic and entities
├── Application/         # Use cases, commands, and queries
├── Infrastructure/      # External concerns implementation
│   ├── Data/            # EF Core repositories for CUD operations
│   └── DaprServices/    # Dapr implementations for read operations
├── WebApi/              # API endpoints and configuration
│   ├── GraphQL/         # GraphQL schema, queries, and mutations
│   └── Rest/            # REST controllers and endpoints
└── ApiGateway/          # Gateway to route between GraphQL and REST
```

## Domain Model

The domain layer is fully implemented with the following key components:

### Core Entities

- **Course**: Main educational offering with modules and lessons
- **User**: Base entity for authentication and authorization
  - **Student**: User who enrolls in courses
  - **Instructor**: User who creates and manages courses
- **Enrollment**: Student's participation in a course
- **Module**: Organizational unit within a course
- **Lesson**: Individual learning unit
- **Assignment**: Evaluation task for students
- **Submission**: Student's completed assignment
- **Progress**: Tracking of student advancement through lessons

### Value Objects

- **Email**: Validates email format
- **Duration**: Represents time spans for courses and lessons
- **Rating**: Manages course ratings with validation
- **Various Enumerations**: CourseStatus, CourseLevel, UserRole, etc.

### Domain Events

Events that trigger system responses, such as:

- CourseCreatedEvent
- EnrollmentCreatedEvent
- SubmissionGradedEvent

### Key Domain Rules

- Courses must have at least one module before publishing
- Students can only submit assignments for courses they're enrolled in
- Courses can only be rated after completion
- Submissions cannot be updated after being graded

## Current Status

- ✅ Domain layer fully implemented with entities, value objects, and interfaces
- ✅ Design follows DDD aggregates, entities, value objects, and repositories
- ✅ Business rules encapsulated within domain entities

## Next Steps

- [ ] Implement Infrastructure layer with database context and repositories
- [ ] Implement Application layer with commands and queries
- [ ] Set up GraphQL schema with queries and mutations
- [ ] Configure REST API endpoints for alternative access
- [ ] Implement Dapr integration for read operations
- [ ] Configure Entity Framework Core for create, update, and delete operations
- [ ] Set up dual API gateway to route between GraphQL and REST
- [ ] Add authentication and authorization
- [ ] Implement real-time notifications for course updates
- [ ] Add file upload functionality for submissions

## Getting Started

### Prerequisites

- .NET Core SDK 7.0 or higher
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository

```bash
git clone https://github.com/yourusername/e-learning-platform.git
```

2. Open the solution in Visual Studio or VS Code

3. Set up the database (instructions to be added)

4. Run the application

```bash
dotnet run --project src/WebApi
```

## Project Structure Explanation

The project follows Clean Architecture and Domain-Driven Design principles:

### Domain Layer (Implemented)

The core of the application containing:

- Business entities
- Domain events
- Repository interfaces
- Value objects
- Domain services

This layer is independent of external frameworks and contains the core business logic.

### Application Layer (Coming Soon)

Will contain:

- Command/Query handlers
- DTOs
- Validation rules
- Interface adapters

### Infrastructure Layer (Coming Soon)

Will contain:

- Database implementation
- External service integrations
- Repository implementations
- Authentication providers

### WebApi Layer (Coming Soon)

Will contain:

- GraphQL schema and resolvers for flexible queries
- REST controllers for traditional API endpoints
- Middleware configuration
- API documentation

### API Implementation Strategy

The API follows a hybrid approach:

- **Read Operations**: Implemented using Dapr for distributed, scalable queries
- **Create, Update, Delete Operations**: Implemented using Entity Framework Core for transactional consistency
- **Dual Interface**: Both GraphQL and REST APIs exposing the same underlying functionality
  - GraphQL for flexible, client-specific data retrieval
  - REST for simpler, resource-focused operations

## Contributing

Instructions for contributing to the project will be added here.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
