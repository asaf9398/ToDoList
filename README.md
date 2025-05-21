# ToDo List App – Real-Time Multi-User System

This project is a WPF-based ToDo application connected to a .NET 8 Web API server using SignalR for real-time communication and Entity Framework Core for SQL Server database interaction.

---

## Project Structure

- **ToDoListClient**: WPF application using MVVM pattern
- **ToDoListServer**: ASP.NET Core Web API + SignalR
- **Common**: Shared DTOs and Enums

---

## Features

- Add, edit, and delete tasks
- Lock tasks to prevent concurrent edits
- Real-time updates via SignalR
- Tasks sorted by priority and completion
- Tasks marked completed are colored and pushed down
- Show task creation time
- Mark task as completed from main screen
- Fully async operations
- Designed with extensibility and clarity in mind

---

## Technologies

- .NET 8 (ASP.NET Core, WPF)
- Entity Framework Core (Code First)
- SignalR (real-time messaging)
- SQL Server (local or remote)
- MSTest (optional – for unit testing)

---

## Installation Guide

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio (For this project I used 2022...)](https://visualstudio.microsoft.com/downloads/)

---

## Setting up the Database

### Option 1: Using SQL Script

Use the following SQL to manually create the `Tasks` table:

```sql
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'ToDoDb')
BEGIN
    CREATE DATABASE ToDoDb;
END
GO

USE ToDoDb;
GO

CREATE TABLE Tasks (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Title NVARCHAR(MAX) NOT NULL,
    Description NVARCHAR(MAX) NOT NULL,
    Priority INT NOT NULL,
    IsCompleted BIT NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    LockedBy NVARCHAR(MAX),
    LockTimestamp DATETIME2
);
GO

CREATE TABLE TaskAuditLogs (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TaskId UNIQUEIDENTIFIER NOT NULL,
    Action NVARCHAR(MAX) NOT NULL,
    Username NVARCHAR(MAX) NOT NULL,
    Timestamp DATETIME2 NOT NULL,
    Description NVARCHAR(MAX)
);
GO

```

### Option 2: Using EF Core Migrations

1. Navigate to the server project directory:

   ```
   cd ToDoListServer
   ```

2. Run:
   ```
   dotnet ef database update
   ```

> Ensure your `appsettings.json` has a valid SQL Server connection string.

---

## Running the Application

### 1. Run the Server

```
dotnet run --project ToDoListServer
```

Server will start on: `http://localhost:5050`

### 2. Run the Client

You can run the WPF project from Visual Studio (`ToDoListClient`) or via:

```
dotnet run --project ToDoListClient
```

---

## Communication Protocol Choice & Reasoning

- This application uses a combination of RESTful API and SignalR (WebSocket-based real-time communication):
### RESTful API is used for:
- Creating, updating, deleting, and retrieving tasks.
- Ensures clear, stateless, and testable endpoints.
- Easily debuggable and compatible with various clients (e.g., Postman, curl).
 
### SignalR is used for:
- Real-time updates (when tasks are added, updated, deleted, or locked).
- Enables push-based updates to all connected clients without polling.
- Improves UX by ensuring that all users are synchronized automatically.

### Reasoning:
- REST is ideal for standard CRUD operations, while SignalR complements it by handling collaborative, real-time events (like task locking/editing notifications), which improves responsiveness and prevents conflicts in multi-user scenarios.

## Design Patterns Used
### Several key design patterns were applied in this solution:

 MVVM (Model-View-ViewModel) – in the WPF Client:
- Ensures separation of concerns: UI (XAML), business logic (ViewModel), and data (Model).
- Simplifies testing and maintainability.
- Enables powerful data binding.
 
 Command Pattern – via ICommand in WPF:
- Commands (RelayCommand) are bound to UI buttons for Add/Delete operations.
- Enables encapsulating user actions and handling UI logic cleanly from the ViewModel.
- Observer Pattern (via SignalR)
- The client "subscribes" to server-pushed updates.
- Enables real-time propagation of state changes to all observers (clients).
 
 Repository Pattern – in the Server Backend:
- Abstracts database access logic behind interfaces.
- Promotes decoupling and simplifies mocking in unit testing.
- Makes the codebase more modular and testable.

 Observer Pattern (SignalR):
- SignalR enables a publish/subscribe mechanism — the server notifies all subscribed clients of changes using Clients.All.SendAsync(...).
- Enables real-time updates to clients without polling.

 DTO Pattern (Data Transfer Object):
- Instead of exposing the database entity directly, the system uses TaskDto to transfer only necessary data.
- Enhances security, performance, and allows shaping the response as needed.

## Demonstration video link(Youtube)
- [Youtube](https://youtu.be/QRMGNnEU-Uo)

## Credits
Project by Asaf Yosef
Assignment: CityShob .NET WPF + Web API Homework  
May 2025

