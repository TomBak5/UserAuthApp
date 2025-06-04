# User Authentication Application

A full-stack web application providing user authentication functionality with ASP.NET Core backend and React TypeScript frontend.

## Features

- User registration with validation
- User login with secure authentication
- User profile management
- List of registered users
- Modern Material-UI interface
- Secure password hashing
- Input validation and error handling
- Detailed error logging

## Tech Stack

### Backend
- ASP.NET Core 9.0
- Entity Framework Core
- SQL Server
- BCrypt for password hashing
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
   The API will be available at `http://localhost:5158`

### Frontend Setup

1. Navigate to the client project:
   ```bash
   cd client
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm start
   ```
   The application will be available at `http://localhost:3000` (or next available port)

## API Endpoints

- POST `/api/Auth/register` - Register a new user
- POST `/api/Auth/login` - Login user
- GET `/api/Auth/users` - Get list of users

## Project Structure

```
UserAuthApp/
├── UserAuthApp.Api/     # Backend API
├── client/             # Frontend React application
└── README.md
```

## Validation Rules

### Registration
- Password must be at least 6 characters long
- Phone number must be in a valid format
- Email must be unique and in valid format
- All required fields must be filled

## Security Features

- Password hashing using BCrypt
- Input validation and sanitization
- CORS configuration
- Error handling middleware
- Secure HTTP headers
- Detailed error logging

## Development

1. Backend development:
   - Use Visual Studio 2022 or VS Code
   - API documentation available at `/swagger`
   - Entity Framework for database operations

2. Frontend development:
   - Material-UI components for modern UI
   - TypeScript for type safety
   - React Router for navigation
   - Axios for API communication
   - Comprehensive error logging

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details 