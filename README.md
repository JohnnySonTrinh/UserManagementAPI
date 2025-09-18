# User Management API (ASP.NET Core)

## Overview
This project is a simple User Management API built with ASP.NET Core. It provides endpoints to create, read, update, and delete user records.

## Features
- Full CRUD operations for users
- In-memory data storage
- Data validation for user input
- Simple password-protected access (X-Api-Password header)
- Middleware for error handling and request logging
- Interactive API documentation via Swagger UI

## Usage
- Start the API with `dotnet run`
- Access Swagger UI at `/swagger` for testing endpoints
- Use the header `X-Api-Password: 12345` to access protected endpoints

## Purpose
This project demonstrates clean, testable ASP.NET Core API design with middleware and validation. It is ideal for learning, prototyping, or as a foundation for more advanced user management systems.
