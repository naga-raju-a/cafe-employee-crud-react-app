This README is designed to be clear, informative, and helpful for any developer looking to understand, set up, and contribute to your project.

---

# Cafe Employee API

A simple and well-structured RESTful API built with ASP.NET Core for managing cafes and their employees. This project demonstrates a clean architecture using Repository and Service layers for separation of concerns.

## Table of Contents

- [Features](#features)
- [Project Architecture](#project-architecture)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Setup and Installation](#setup-and-installation)
- [API Endpoints](#api-endpoints)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## Features

- ✅ **CRUD Operations** for Cafes.
- ✅ **CRUD Operations** for Employees.
- ✅ **List Employees** by a specific Cafe.
- ✅ **Data Seeding** for initial setup with sample data.
- ✅ **Swagger/OpenAPI** documentation for easy API exploration and testing.
- ✅ **Clean Architecture** using Repository and Service patterns.
- ✅ **Entity Framework Core** for data access and migrations.

## Project Architecture

This project follows a clean, n-tier architecture to ensure separation of concerns, maintainability, and testability.

- **Controllers**: Handle incoming HTTP requests and route them to the appropriate service. They are the entry point to the API.
- **Services**: Contain the core business logic. They orchestrate operations by coordinating with repositories.
- **Repositories**: Abstract the data access layer. They are responsible for querying and persisting data, keeping the data access logic separate from the business logic.
- **DTOs (Data Transfer Objects)**: Custom objects used to transfer data between layers (e.g., between the service layer and the controllers/client) to prevent exposing internal domain models.
- **Models**: Represent the core domain entities and map directly to the database tables via Entity Framework Core.

## Technology Stack

- **[.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)** (or your specific version)
- **ASP.NET Core Web API**
- **Entity Framework Core 8** - ORM for database interaction.
- **SQL Server** - (Default, but can be easily swapped for another provider like PostgreSQL or SQLite).
- **Swagger (Swashbuckle)** - For API documentation and testing.

## Getting Started

Follow these instructions to get a local copy of the project up and running.

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (Version 8.0 or later recommended)
- A code editor like [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [SQL Server Express](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or another database instance.

### Setup and Installation

1.  **Clone the repository:**
    ```sh
    git clone https://github.com/your-username/afe-employee-crud-react-app.git
    cd CafeEmployeeApi
    ```

2.  **Configure the database connection:**
    - Open `appsettings.Development.json`.
    - Update the `DefaultConnection` string with your database credentials.

    ```json
    {
      "ConnectionStrings": {
        "DefaultConnection": "<provide connectionstring>"
      },
      "Logging": {
        // ...
      }
    }
    ```
    > **Note:** For production, it's recommended to use User Secrets or another secure configuration provider.

3.  **Restore dependencies:**
    Open a terminal in the project root and run:
    ```sh
    dotnet restore
    ```

4.  **Apply Entity Framework migrations:**
    This command will create the database and tables based on the models and seed it with initial data.
    ```sh
    dotnet ef database update
    ```

5.  **Run the application:**
    ```sh
    dotnet run
    ```
    The API should now be running. By default, you can access it at `https://localhost:7123` or `http://localhost:5123`.

6.  **Explore the API with Swagger:**
    Navigate to `https://localhost:7123/swagger` in your browser to view the interactive API documentation.

## API Endpoints

Here are the primary endpoints available in this API.

### Cafes (`/api/cafes`)

| Method | Endpoint                    | Description                                  |
| :----- | :-------------------------- | :------------------------------------------- |
| `GET`  | `/`                         | Get a list of all cafes.                     |
| `GET`  | `/{id}`                     | Get a single cafe by its unique ID.          |
| `POST` | `/`                         | Create a new cafe.                           |
| `PUT`  | `/{id}`                     | Update an existing cafe.                     |
| `DELETE`| `/{id}`                     | Delete a cafe by its unique ID.              |
| `GET`  | `?location={location}`      | Get cafes filtered by location.              |

### Employees (`/api/employees`)

| Method | Endpoint         | Description                                    |
| :----- | :--------------- | :--------------------------------------------- |
| `GET`  | `/`              | Get a list of all employees.                   |
| `GET`  | `/{id}`          | Get a single employee by their unique ID.      |
| `POST` | `/`              | Create a new employee.                         |
| `PUT`  | `/{id}`          | Update an existing employee.                   |
| `DELETE`| `/{id}`          | Delete an employee by their unique ID.         |

## Project Structure

```
/CafeEmployeeApi
├── Controllers/            # API controllers (entry points)
│   ├── CafesController.cs
│   └── EmployeesController.cs
├── Data/                   # EF Core DbContext and data seeding
│   ├── AppDbContext.cs
│   └── SeedData.cs
├── DTOs/                   # Data Transfer Objects
│   ├── CafeDto.cs
│   ├── CreateOrUpdateCafeDto.cs
│   ├── CreateOrUpdateEmployeeDto.cs
│   └── EmployeeDto.cs
├── Models/                 # Domain entities (database models)
│   ├── Cafe.cs
│   └── Employee.cs
├── Repositories/           # Data access logic (interfaces and implementations)
│   ├── CafeRepository.cs
│   ├── EmployeeRepository.cs
│   ├── ICafeRepository.cs
│   └── IEmployeeRepository.cs
├── Services/               # Business logic (interfaces and implementations)
│   ├── CafeService.cs
│   ├── EmployeeService.cs
│   ├── ICafeService.cs
│   └── IEmployeeService.cs
├── Migrations/             # EF Core generated migration files
├── appsettings.json        # Application configuration
├── CafeEmployeeApi.csproj  # Project file
└── Program.cs              # Application startup and service registration
```

## Contributing

Contributions are welcome! If you have suggestions for improving the API, please feel free to create an issue or submit a pull request.

1.  Fork the Project.
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`).
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`).
4.  Push to the Branch (`git push origin feature/AmazingFeature`).
5.  Open a Pull Request.

## License

This project is licensed under the MIT License. See the `LICENSE` file for more information.
