This README organizes the installation steps, explains the project's purpose, and includes the specific troubleshooting information you provided.

---

# Cafe & Employee Management UI (React)

This is a responsive web application built with React to manage cafes and their employees. It serves as the frontend for the **[Cafe Employee API](https://github.com/naga-raju-a/cafe-employee-crud-react-app/CafeEmployeeApi)**, providing a user-friendly interface for all CRUD operations.

The application features a clean design using Material-UI (MUI) and powerful data tables from AG Grid.

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
- [Available Scripts](#available-scripts)
- [Project Structure](#project-structure)
- [Troubleshooting](#troubleshooting)
  - [`npm run build` Fails to Minify](#npm-run-build-fails-to-minify)
- [Contributing](#contributing)
- [License](#license)

## Features

- ✅ **Full CRUD Functionality**: Create, Read, Update, and Delete cafes.
- ✅ **Employee Management**: Create, Read, Update, and Delete employees associated with cafes.
- ✅ **Interactive Data Tables**: View, sort, and filter data using AG Grid.
- ✅ **Responsive Design**: Modern and responsive UI built with Material-UI (MUI).
- ✅ **Client-Side Routing**: A seamless single-page application (SPA) experience using React Router.
- ✅ **Dedicated Forms**: User-friendly forms for adding and editing data with validation.

## Technology Stack

This project is built with a modern frontend stack:

- **React**: A JavaScript library for building user interfaces.
- **React Router**: For declarative routing within the application.
- **Material-UI (MUI)**: A comprehensive suite of UI tools to build React applications faster.
- **AG Grid**: A feature-rich data grid/table component for React.
- **Axios**: A promise-based HTTP client for making API requests to the backend.
- **Day.js**: A lightweight JavaScript date library for handling dates in the UI.
- **Create React App**: Used to set up the initial project structure and scripts.

## Getting Started

Follow these instructions to get a local copy of the project up and running for development.

### Prerequisites

1.  **Node.js and npm**: Make sure you have Node.js (v16 or later) and npm installed. You can download them from [nodejs.org](https://nodejs.org/).
2.  **Running Backend API**: This frontend requires the [Cafe Employee API](https://github.com/naga-raju-a/cafe-employee-crud-react-app/CafeEmployeeApi) to be running, as it fetches and sends data to it. Ensure the backend is running locally (usually on `https://localhost:<port>`).

### Installation

1.  **Clone the repository:**
    ```sh
    git clone https://github.com/naga-raju-a/cafe-employee-crud-react-app.git
    cd cafe-employee-react-ui
    ```

2.  **Configure the API endpoint:**
    The application needs to know the base URL of your backend API. Create a `.env` file in the root of the project directory:
    ```sh
    touch .env
    ```
    Add the following line to the `.env` file. Adjust the URL if your API is running on a different port.
    ```
    REACT_APP_API_BASE_URL=https://localhost:<port>/api
    ```
    The `cafeService.js` and `employeeService.js` are configured to use this environment variable.

3.  **Install dependencies:**
    This command installs all the necessary libraries and packages listed in `package.json`.
    ```sh
    npm install
    ```

4.  **Start the development server:**
    ```sh
    npm start
    ```
    This will run the app in development mode. Open [http://localhost:3000](http://localhost:3000) to view it in your browser. The page will automatically reload when you make changes.

## Available Scripts

In the project directory, you can run the following scripts:

-   `npm start`
    -   Starts the development server on `http://localhost:3000`.

-   `npm test`
    -   Launches the test runner in interactive watch mode.

-   `npm run build`
    -   Bundles the app into static files for production in the `build` folder.

-   `npm run eject`
    -   **Note:** This is a one-way operation. Once you `eject`, you can’t go back! It removes the single build dependency from your project and copies all configuration files and scripts directly into your project.

Of course! Here is the updated **Project Structure** section for your React README.md, reflecting the new, more organized component-based architecture.

---

## Project Structure

This project follows a component-based architecture, promoting reusability and separation of concerns.

```
/project
├── src/
│   ├── components/                 # React components
│   │   ├── CafePage.js             # Main page for displaying and managing cafes
│   │   ├── EmployeePage.js         # Main page for displaying and managing employees
│   │   └── common/                 # Shared, reusable UI components
│   │       ├── EmployeeDialog.js   # Modal dialog for creating/editing an employee
│   │       ├── CafeDialog.js       # Modal dialog for creating/editing a cafe
│   │       ├── ConfirmDialog.js    # Generic dialog for confirming actions (e.g., delete)
│   │       └── ReusableTextField.js# Standardized text field for forms
│   │
│   ├── services/                   # Logic for communicating with the backend API
│   │   └── app.js                  # Consolidates all API service functions
│   │
│   ├── util/                       # Helper and utility functions
│   │   └── validation.js           # Reusable form validation logic
│   │
│   ├── App.js                      # Main application component with routing
│   └── index.js                    # Entry point of the React application
│
├── public/                         # Public assets
└── package.json                    # Project dependencies and scripts
```

### Directory Breakdown

-   **`src/components`**: This directory contains all the React components.
    -   **Page Components** (`CafePage.js`, `EmployeePage.js`): These are the main "view" components that orchestrate the display of data tables and user interaction buttons.
    -   **`common/`**: This sub-directory holds highly reusable components that are shared across different parts of the application, such as modal dialogs for forms and confirmation prompts.

-   **`src/services`**: Handles all communication with the backend API.
    -   `app.js`: Consolidates all Axios-based functions for fetching, creating, updating, and deleting data for both cafes and employees.

-   **`src/util`**: Contains utility functions that are not specific to any single component.
    -   `validation.js`: Houses the logic for validating form inputs, keeping validation rules separate from the UI components.

-   **`src/App.js`**: The root component of the application. It sets up the client-side routing using `react-router-dom`.

-   **`src/index.js`**: The main entry point that renders the `App` component into the DOM.

## Troubleshooting

### `npm run build` Fails to Minify

**Problem:**
When running `npm run build`, you might encounter an error related to minification, often caused by dependency conflicts within `react-scripts`.

**Cause:**
This is a known issue with older versions of `react-scripts` and its transitive dependencies, particularly `ajv` (Another JSON Schema Validator).

**Solution:**
You can resolve this by explicitly installing a compatible version of `ajv` in your project's `devDependencies`.

1.  Run the following command:
    ```sh
    npm install ajv@8 --save-dev
    ```

2.  (Optional) If the problem persists, try cleaning the npm cache before reinstalling:
    ```sh
    npm cache clean --force
    npm install
    ```

3.  Now, try running the build command again:
    ```sh
    npm run build
    ```

## Contributing

Contributions are welcome! Please feel free to fork the repository, make changes, and submit a pull request.

1.  Fork the Project.
2.  Create your Feature Branch (`git checkout -b feature/NewComponent`).
3.  Commit your Changes (`git commit -m 'Add some NewComponent'`).
4.  Push to the Branch (`git push origin feature/NewComponent`).
5.  Open a Pull Request.

## License

This project is licensed under the MIT License.