# User Authentication Application

A full-stack web application providing user authentication functionality with ASP.NET Core backend and React TypeScript frontend.

## Features

- User registration with validation
- User login with secure authentication
- User profile management
- List of registered users
- Modern Material-UI interface
- Secure password hashing
- Input validation
- Error handling
- Unit and integration tests

## Tech Stack

### Backend
- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server
- BCrypt for password hashing
- xUnit for testing
- Swagger/OpenAPI

### Frontend
- React 18
- TypeScript
- Material-UI
- Axios
- React Router

## Prerequisites

- .NET 9.0 SDK
- Node.js and npm
- SQL Server
- Visual Studio 2022 or VS Code

## Setup Instructions

### Backend Setup

1. Clone the repository
2. Navigate to the API project:
   ```bash
   cd UserAuthApp.Api
   ```
3. Update the connection string in `appsettings.json` with your SQL Server details
4. Apply database migrations:
   ```bash
   dotnet ef database update
   ```
5. Run the backend:
   ```bash
   dotnet run
   ```
   The API will be available at `https://localhost:7001`

### Frontend Setup

1. Navigate to the frontend project:
   ```bash
   cd UserAuthApp.Client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Update the API URL in `src/services/api.ts` if needed
4. Start the development server:
   ```bash
   npm start
   ```
   The application will be available at `http://localhost:3000`

## Running Tests

### Backend Tests
```bash
cd UserAuthApp.Tests
dotnet test
```

### Integration Tests
```bash
cd UserAuthApp.IntegrationTests
dotnet test
```

## API Endpoints

- POST `/api/Auth/register` - Register a new user
- POST `/api/Auth/login` - Login user
- GET `/api/Auth/users` - Get list of users

## Project Structure

```
UserAuthApp/
├── UserAuthApp.Api/           # Backend API
├── UserAuthApp.Client/        # Frontend React application
├── UserAuthApp.Tests/         # Unit tests
└── UserAuthApp.IntegrationTests/  # Integration tests
```

## Security Features

- Password hashing using BCrypt
- Input validation and sanitization
- CORS configuration
- Error handling middleware
- Secure HTTP headers

## Development

1. Backend development:
   - Use Visual Studio 2022 or VS Code
   - API documentation available at `/swagger`
   - Entity Framework migrations for database changes

2. Frontend development:
   - Material-UI components
   - TypeScript for type safety
   - React Router for navigation
   - Axios for API communication

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details 