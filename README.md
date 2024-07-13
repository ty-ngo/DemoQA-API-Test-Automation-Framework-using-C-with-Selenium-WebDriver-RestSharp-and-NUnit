# DemoQA API Automation Test Project

## Overview
This project is an automation test suite for the DemoQA, developed using .NET 8 with C# as the primary programming language, and NUnit 3 for testing.

## Project Structure
The solution comprises three projects:

1. **Core**: Contains utilities for API interaction, configuration file management, status code verification, schema validation, and data storage for teardown processes.
2. **DemoQA.Services**: Contains request and response models, as well as `UserServices` and `BookServices` for sending API requests.
3. **DemoQA.APITesting**: Contains the test cases for `UserServices` and `BookServices`, test data, and configuration settings (`appsettings.json`).

## Development Tools
The project is set up using Visual Studio Code.

## Configuration File
The primary configuration file for this project is `appsettings.json`, which contains the application URL

## Running Tests
Before running the test, you must log into https://demoqa.com/books to create new user and get the username, password, userId. Then, paste it into the file account.json in the `DemoQA.APITesting/TestData/Account` folder.
### Using Visual Studio Code
- Utilize the Test Explorer to select and run tests.

### Using Command Line
1. Restore all dependency packages:
   ```sh
   dotnet restore
   ```
2. Build the project:
   ```sh
   dotnet build
   ```
3. Run all tests:
   ```sh
   dotnet test
   ```
4. Run specific tests based on category:
   ```sh
   dotnet test --filter Category=AddBook
   ```
   (Replace `AddBook` with the desired test category: `GetUser`, `DeleteBook` or `ReplaceBook`)