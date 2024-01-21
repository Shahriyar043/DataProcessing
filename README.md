# DataProcessing.Api Readme
## Project Overview
DataProcessing.Api is a .NET Core 8.0 project designed to facilitate data processing through a set of APIs utilizing RabbitMQ. The primary functionalities of this project include reading data from a CSV file, sending data to a RabbitMQ queue, and receiving data from the queue.

## Prerequisites
Before running the application, ensure that the following prerequisites are installed on your system:

## Docker: Install Docker
.NET Core 8.0: Install .NET Core
## RabbitMQ Setup
To run RabbitMQ in a Docker container, execute the following commands:

```bash
docker pull rabbitmq
docker run -d --hostname rmq --name rabbit-server -p 8080:15672 -p 5672:5672 rabbitmq:3-management
  ```

This will pull the RabbitMQ image from Docker Hub and run it with the specified configurations.

## API Endpoints

Read Data from File
Endpoint: POST https://localhost:7298/api/v1.0/data/read-data-from-file
Request:
Method: POST
Content-Type: multipart/form-data
Body: Attach a CSV file using the "file" parameter.
Response: The API processes the CSV file and returns a success response or an error message.

Send Data to Queue
Endpoint: POST https://localhost:7298/api/v1.0data/send-data-to-queue
Method: POST
Content-Type: application/json
Response: The API sends the data to the RabbitMQ queue and returns a success response or an error message.

Receive Data from Queue
Endpoint: POST https://localhost:7298/api/v1.0data/receive-data-from-queue
Method: POST
Content-Type: application/json
Response: The API retrieves data from the RabbitMQ queue and returns it in the response.

```bash
cd DataProcessing.Api
```

## Build and run the application:

```bash
dotnet build
dotnet run
```
The application will start, and you can access the APIs as described above.

## Additional Notes
Ensure that RabbitMQ is running before using the APIs.
Customize the RabbitMQ connection settings in the appsettings.json file if needed.