# Inventory System (Sample Project)

This is a sample ASP.NET Core backend project demonstrating:

- Clean Architecture
- Repository Pattern
- Dependency Injection (DI)
- Test-Driven Development (TDD) using xUnit and Moq

> This project was created by **Ruel Sucgang** as part of a developer portfolio.  
> **For demo/viewing purposes only**. Not intended for commercial use.

## Project Structure
Inventory.API --> ASP.NET Core Web API (entry point)
Inventory.Application --> Business logic, use cases, services
Inventory.Core --> Domain models and interfaces
Inventory.Infrastructure --> Data access (EF Core)
Inventory.Tests --> xUnit unit tests with Moq

## Sample TDD Implementation

Implemented TDD for:
- `ChangePassword` method in `UserService`
- Used Moq to isolate `IUserRepository`
- Unit tests in `UserServiceTests.cs`

## Work in Progress

This project will be expanded to include:
- Redis caching
- PostgreSQL via Supabase or external DB
- GitHub Actions CI/CD pipeline

## License

This project is licensed under the [MIT License](LICENSE).  
© 2025 Ruel Sucgang. All rights reserved.

## Author

**Ruel Sucgang**  
Senior .NET Developer | Clean Architecture Advocate  
