# Backend Instructions
The backend server is built using ASP.NET Core Minimal API. It provides endpoints for product management. Below are instructions on how to set up and run the server.

## Running the Server
1. Navigate to the `reko-mini-project.Server` directory in your terminal.
2. Run this command to start the server:
   ```
   dotnet run
   ```
3. The server will start listening on the configured port (default is 7213). You should see output in the terminal indicating that the server is running:
   ```
   Now listening on: https://localhost:7213
   ```

## API Endpoints
- `GET /products`: Currently prints "Hello Reko" (will be updated with appropriate data management soon).
- *TODO: Add more endpoints as needed (e.g., POST /products, GET /products/{id}, etc.)*

### Curl testing
You can test the API endpoints using curl commands in your terminal. For example, to test the GET /products endpoint, run:
```
curl https://localhost:7213/api/products
```

### Scalar testing
To use Scalar, add /Scalar to the server URL in the browser:
```
https://localhost:7213/Scalar
```
Select the endpoint you want to test from the list, locate the "Test Request" button, click it, then click the "Send" button.

## CORS Configuration
The server is configured to allow cross-origin requests from the frontend application. The allowed origins are specified in launchSettings.json under the `ALLOWED_ORIGINS` environment variable.
Ensure that the frontend application is running on the allowed origin specified in the server configuration to avoid CORS issues.

## Database Configuration
The server uses Entity Framework Core with SQLite for data storage.
*(Will be replaced with a more robust database solution in the future - possibly Azure SQL database.)*
 The connection string is defined in the appsettings.Development.json file under the `ConnectionStrings` section. The default connection string points to a local SQLite database file located at `Data/inventorydata.db`.

Run the following command to create the database file. The file will be created in the Data folder, as "inventorydata.db":
```
dotnet ef database update
```
Now you should be able to run the server and it will use the SQLite database for data storage. Manipulate the database using Scalar endpoints to add, update, or delete data. The data is purely local at this stage, so it will not persist across different machines or deployments.
