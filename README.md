# Full-Stack Cafe & Employee Management Application

This repository contains the source code for a full-stack web application designed to manage cafes and their employees. The project is separated into a .NET Web API backend and a React frontend client.

## Project Structure

The repository is organized into two primary folders, one for the backend API and one for the frontend client. Each folder contains its own detailed `README.md` with specific setup instructions.

-   **`/CafeEmployeeApi`**: The .NET 8 Web API backend.
    -   **[➡️ Go to Backend README for setup instructions](./CafeEmployeeApi/README.md)**

-   **`/cafe-employee-react-ui`**: The React.js frontend application.
    -   **[➡️ Go to Frontend README for setup instructions](./cafe-employee-react-ui/README.md)**

## High-Level Architecture

The application follows a classic client-server architecture where the React client consumes the RESTful .NET API.

```
[Browser] <--> [React Frontend] <--> [ASP.NET Core API] <--> [SQL Database]
```

## Technology Stack

-   **Backend:** .NET 8, ASP.NET Core Web API, Entity Framework Core, SQL Server (SQLlite)
-   **Frontend:** React, Material-_UI (MUI), AG Grid, Axios
-   **API Documentation:** Swagger/OpenAPI

## Getting Started

To run the full application, you need to set up and run both the backend API and the frontend client simultaneously.

### 1. Run the Backend API

Navigate to the backend folder, follow its setup instructions, and start the server.

```sh
# In the /CafeEmployeeApi directory
dotnet run
```
> The API will typically be available at `https://localhost:<port>`.

### 2. Run the Frontend Client

In a **new terminal window**, navigate to the frontend folder, follow its setup instructions, and start the client.

```sh
# In the /cafe-employee-react-ui directory
npm start
```
> The React application will open in your browser at `http://localhost:3001`.

---

For detailed information on dependencies, project structure, and specific configurations, please refer to the `README.md` file inside each respective project folder.
