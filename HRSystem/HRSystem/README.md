# HR System API

A pre-built ASP.NET Core Web API that simulates an enterprise HR system.  
This is provided as-is for other systems (such as the Quotation Module) to consume via REST API calls.

## Prerequisites

- Visual Studio 2022 (any edition)
- .NET 8 SDK

> **No database server required.** This project uses SQLite — the database is a single  
> file (`HRSystem.db`) created automatically in the project folder on first run.

## Setup Instructions

1. Open `HRSystem.sln` in Visual Studio.
2. Build the solution (`Ctrl + Shift + B`).
3. Run the project (`F5` or `Ctrl + F5`).
4. The API will start and open Swagger UI in your browser.

> The database is **automatically created and seeded** with sample data on first run.  
> No migrations or manual setup required.

## Swagger UI

Once running, navigate to:

```
https://localhost:{port}/swagger
```

This provides an interactive page where you can test every endpoint.

## Quick Test

With the API running, open a browser and go to:

```
https://localhost:{port}/api/employees
```

You should see a JSON array of 12 employees.

## Default Port

The default port is assigned by Visual Studio. Check the `launchSettings.json`  
or the console output when you run the project.

## Connection String

The default connection string uses **SQLite**:

```
Data Source=HRSystem.db
```

This is pre-configured in `appsettings.json`. The database file is created
automatically in the project folder — no server installation needed.
