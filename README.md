# TimeCraft - .NET API Backend

## Overview

Welcome to the TimeCraft application, a powerful Time Management tool designed to help users organize their tasks, schedule events, and optimize their daily routines. This README specifically covers the .NET API backend of TimeCraft.

## Table of Contents

1. [Getting Started](#getting-started)
2. [Prerequisites](#prerequisites)
3. [Installation](#installation)
4. [Configuration](#configuration)
5. [Usage](#usage)
6. [API Endpoints](#api-endpoints)
7. [Authentication](#authentication)
8. [Contributing](#contributing)
9. [License](#license)

## Getting Started

These instructions will help you set up and run the TimeCraft .NET API backend on your local machine or in a production environment.

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) installed
- [Microsoft SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) for database (optional: you can configure another supported database)
- Code editor such as [Visual Studio](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)

## Installation

1. Clone the TimeCraft repository:

   ```bash
   git clone https://github.com/TimeCraft-App/Back-end.git
   ```
2. Navigate to the TimeCraft/Back-end directory:
    ```bash 
    cd TimeCraft/Back-end
    dotnet run
    ```
The API backend should now be running locally.

Configuration
Configure the application by updating the appsettings.json file with your database connection string, authentication settings, and other relevant configurations.
```
json
Copy code
{
  "ConnectionStrings": {
    "DefaultConnection": "your_database_connection_string"
  },
  "Authentication": {
    "SecretKey": "your_secret_key",
    "Issuer": "your_issuer",
    "Audience": "your_audience"
  },
  // ... other configurations
}
```
### API Endpoints
Document the available API endpoints, their input parameters, and expected responses. Include examples where applicable.

GET /api/tasks: Retrieve all tasks.
GET /api/tasks/{id}: Retrieve a specific task by ID.
POST /api/tasks: Create a new task.
json
Copy code
{
  "title": "Task Title",
  "description": "Task Description",
  // ... other parameters
}
PUT /api/tasks/{id}: Update an existing task by ID.
json
Copy code
{
  "title": "Updated Task Title",
  "description": "Updated Task Description",
  // ... other parameters
}
DELETE /api/tasks/{id}: Delete a task by ID.
### Authentication
Explain the authentication mechanism used in the API (e.g., JWT tokens) and how users can obtain and include authentication tokens in their requests.


### Contributing
The ones interested in contributing to this project must email me at jetonsllamniku@gmail.com in order to add as a collaborator in the project

## License

This project is licensed under the [Apache License 2.0](LICENSE). See the LICENSE file for details.
