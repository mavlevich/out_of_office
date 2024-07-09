# Out Of Office

## Description

**Out Of Office** is an application for managing leave requests, projects, and employees. The application allows employees to submit leave requests, and managers to approve or reject them. The system also allows for managing projects and users.

## Features

- User management (create, edit, delete)
- Leave request management (create, approve, reject)
- Project management (create, edit, delete)
- User authentication and authorization
- Data sorting (employees, requests, projects)

## Technologies

- ASP.NET Core
- Entity Framework Core
- SQLite
- Identity
- Swagger
- JWT Authentication


## Configure the Database

Open the appsettings.json file and configure the SQLite database connection string:

"ConnectionStrings": {
  "DefaultConnection": "Data Source=out_of_office.db"
}

## Apply migrations to create the database:

dotnet ef database update

## Run the Application

dotnet run

## Initialize Users and Roles

Run the following command to initialize users and roles:

dotnet ef migrations add InitialCreate
dotnet ef database update
