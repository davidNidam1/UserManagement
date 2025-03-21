# User Management - Backend

## Overview  
This repository contains the backend for a user management system, built using **ASP.NET Core 8** and **MongoDB**.  
The project was developed starting with the backend, before the frontend or testing were implemented. Therefore, API requests were initially tested using **Swagger** before integrating with the frontend and writing dedicated tests.

## Technologies Used  
- **C# with ASP.NET Core 8**  
- **MongoDB** for database management  
- **JWT Authentication**  
- **xUnit** for unit and integration testing  

## Project Structure  
This project is divided into three separate repositories:

### 1. Backend (User Management API)  
```
UserManagement/
│── Controllers/       # API Controllers
│── Models/            # Data Models
│── Services/          # Business Logic
│── Properties/        # Configuration
│── appsettings.json   # Application settings
│── Program.cs         # Main entry point
│── UserManagement.csproj  # Project file
```

### 2. Frontend (User Management Client)  
```
user-management-client/
│── public/            # Static assets
│── src/
│   │── components/     # React components
│   │── pages/          # Application pages
│   │── services/       # API calls
│── index.html         # Main HTML file
│── package.json       # Dependencies
│── vite.config.ts     # Vite configuration
```

### 3. Testing (User Management Tests)  
```
UserServerTests/
│── IntegrationTests/   # API Integration tests
│── UnitTests/         # Unit tests
│── UserServerTests.csproj # Project file
```

## Installation & Setup  

### 1. Clone the repository  
```sh
git clone https://github.com/davidNidam1/UserManagement.git
cd UserManagement
```

### 2. Install dependencies  
Ensure you have **.NET SDK 8** installed. If not, download it from:
[dotnet.microsoft.com](https://dotnet.microsoft.com/en-us/download)

Then, restore dependencies:
```sh
dotnet restore
```

### 3. Configure MongoDB  
By default, the application connects to a local MongoDB instance at:  
```sh
mongodb://localhost:27017
```
If needed, modify the connection string in **appsettings.json**:
```json
"ConnectionStrings": {
  "MongoDb": "mongodb://your-mongodb-url"
}
```
Make sure MongoDB is running before launching the server.

### 4. Run the server  
```sh
dotnet run
```
The server will be available at:
```sh
http://localhost:5089
```

## Related Repositories  
- **Frontend Repository:** [User Management Client](https://github.com/davidNidam1/user-management-client)
- **Testing Repository:** [User Management Tests](https://github.com/davidNidam1/UserServerTests)

## Future Improvements  
If more time were available, the following enhancements could be implemented:
- Improved UI design for a more polished frontend
- Better modular separation for improved maintainability
- Stronger security measures, including rate limiting and OAuth
- Dockerization for easy deployment
- Cloud deployment on AWS, Azure, or DigitalOcean

