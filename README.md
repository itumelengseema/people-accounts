# People Accounts Management System

A simple web application built using **ASP.NET Core MVC** for managing people, their accounts, and financial transactions.

The project focuses on implementing CRUD operations, database relationships, authentication, pagination, and clean code practices using a service-based architecture.

## Features

- User login and cookie-based authentication
- Create, view, update, and delete people
- Search and paginate people records
- Create and manage accounts linked to people
- Record and update account transactions
- View account balances and transaction history
- Validate user input and enforce business rules

## Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 8)
- **Language:** C#
- **Database:** Microsoft SQL Server
- **ORM:** Entity Framework Core
- **Frontend:** Razor Views, HTML, CSS
- **Authentication:** ASP.NET Core Cookie Authentication

## Project Structure

The project follows the MVC pattern with a separate service layer:

- **Controllers** – Handle HTTP requests and responses
- **Models** – Represent database entities
- **Views** – Display the user interface
- **ViewModels** – Transfer data between controllers and views
- **Services** – Contain business logic
- **Data** – Manage database access using Entity Framework Core

Interfaces and dependency injection are used to promote separation of concerns and maintainability.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Microsoft SQL Server
- JetBrains Rider, Visual Studio, or VS Code

### Installation

1. Clone the repository:

   ```bash
   git clone https://github.com/itumelengseema/people-accounts.git
   ```

2. Navigate to the project:

   ```bash
   cd people-accounts
   ```

3. Configure the `DefaultConnection` SQL Server connection string in your local application configuration.

4. Ensure the database schema is created.

5. Run the application:

   ```bash
   dotnet run --project PeopleAccounts.web
   ```


