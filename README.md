## Introduction

This is a task for the "Software Development Trainee" position at ProCredit Bank.<br>The task consists of creating a simple C# Web API that uploads a file that contains a SWIFT (type MT799) message, parses it and uploads the relevant content of it to an SQLite database.

## Features

- Parse MT799 messages
- Extract key data fields
- Return structured JSON responses
- Basic validation of the message format and content

## Technologies Used

- C#
- .NET Core
- ASP.NET Web API
- SQLite
- Swagger for API documentation
- [NLog](https://nlog-project.org) for logging

## Installation

### Prerequisites

- [.NET Core SDK](https://dotnet.microsoft.com/download)
- SQLite

### Steps

1. **Clone the repository:**
    ```bash
    git clone https://github.com/slavi22/ProCredit-task.git
    cd ProCredit-task
    ```

2. **Restore the dependencies:**
    ```bash
    dotnet restore
    ```

3. **Build the project:**
    ```bash
    dotnet build
    ```

4. **Run the application:**
    ```bash
    dotnet run
    ```

## Usage
### API Endpoints

- **Parse MT799 Message**

    **Endpoint:** `POST /api/v1/UploadMessage`

    **Request:**

    The endpoint expects a file upload. The file should contain the MT799 message.

    **Response:**
    ```json
    {
      "id": 0,
      "basicHeaderInfo": "string",
      "applicationHeaderInfo": "string",
      "relatedRef": "string",
      "narrative": "string",
      "mac": "string",
      "chk": "string"
    }
    ```

## Running the project
### When running the project it will open the Swagger UI where you can test the endpoint by uploading a file.
