# Backend Instructions
The backend server is built using ASP.NET Core with Minimal APIs and Entity Framework Core. It provides endpoints for product management and a simple SQLite database for development testing. Below are instructions on how to set up and run the server.

## Running the Server
1. Navigate to the `\reko-mini-project.Server\` directory in your terminal.
2. Run this command to start the server:
```
   dotnet run
```
3. The server will start listening on the configured port (default is 7213). You should see output in the terminal indicating that the server is running:
```
   'Now listening on: https://localhost:7213'
```

## Running Tests
To run the unit tests for the server, open Testing tools in VSCode (View -> Testing Tools) or use the following command in the terminal:
```
   dotnet test
```

## API Endpoints
The server exposes the following API endpoints for product management:
1. `POST /api/products`
   - *Create a new product*.
2. `POST /api/products/{id}/with-image`
   - *Upload an image for a specific product by its ID*.
3. `PUT /api/products/{id}`
   - *Update an existing product by its ID*.
4. `PUT /api/products/{id}/with-image`
   - *Update the image for a specific product by its ID*.
5. `GET /api/products`
   - *Retrieve a list of all products*.
6. `GET /api/products/{id}`
   - *Retrieve a specific product by its ID*.
7. `DELETE /api/products/{id}`
   - *Delete a product by its ID*.
- `POST /api/images/upload`
   - *Upload an image file to blob storage and return the URL of the uploaded image.*


### Scalar testing
To use Scalar, add /Scalar suffix to the server URL in the browser:
```
   https://localhost:7213/Scalar
```
Select the endpoint you want to test from the list, locate the "Test Request" button, click it, then click the "Send" button. 
You may need to fill in some required fields in the request body for certain endpoints before sending the request.

## CORS Configuration
The server is configured to allow cross-origin requests from the frontend application. Allowed origins are specified in launchSettings.json under the `ALLOWED_ORIGINS` environment variable.
Frotend application should run on the allowed origin specified in the server configuration to avoid CORS issues.

## Database Configuration
The server uses ***Entity Framework Core*** with [***`SQLite`***](https://www.sqlite.org) for data storage. The connection string is defined in [***`appsettings.Development.json`***](appsettings.Development.json) under the `ConnectionStrings` section. The default connection string points to a local SQLite database file located in the ***`\Data\`*** folder.

### Resetting existing db file and migrations
If you need to reset the database, delete the existing ***`products.db`*** file in the ***`\Data\`*** folder and remove all migration files in the ***`\Data\Migrations\`*** folder. Then follow the steps below to create a new database.

### Creating the database
To set up the database, run the following command in the terminal to create the initial migration:
```
   dotnet ef migrations add InitialCreate --output-dir .\Data\Migrations\
```
Then run the following command to create the database file. The file will be created in the ***`\Data\`*** folder:
```
   dotnet ef database update
```
Now you should be able to run the server and it will use the SQLite database for data storage. Manipulate the database using Scalar endpoints to add, update, or delete data. The data is purely local at this stage, so it will not persist across different machines or deployments.

## Image upload Blob storage management
Current behavior:
- Image uploads are stored in Azure Blob Storage.
- Auth mode is selected in code by config priority:
   1. `BlobStorage:ConnectionString` -> `BlobServiceClient(connectionString)`
   2. otherwise `BlobStorage:ServiceUri` -> `BlobServiceClient(serviceUri, DefaultAzureCredential)`
- `BlobStorage:ContainerName` is required in all environments.

### Development:
- Use Azurite with `BlobStorage:ConnectionString` in `appsettings.Development.json`.
- Container name is defined in [appsettings.json](appsettings.json) and [appsettings.Development.json](appsettings.Development.json).
- Blob SDK API version is pinned in code for Azurite compatibility.

### Production (Azure Web App/container):
- Recommended auth is Managed Identity (no storage key/connection string in app code).
- Required Azure resources/wiring:
   1. Azure Storage Account with Blob service
   2. Blob container (or allow app to create it)
   3. System- or user-assigned Managed Identity on the Web App
   4. RBAC assignment: `Storage Blob Data Contributor` for that identity on the storage account or container scope
- Required app configuration values:
   - `BlobStorage__ServiceUri=https://<storage-account>.blob.core.windows.net`
   - `BlobStorage__ContainerName=<container-name>`
- Important: do not set `BlobStorage__ConnectionString` in production if you want Managed Identity path.

### Testing:
- Use VSCode extension ***`Azurite`*** for local blob storage emulation. Then you can use these commands in VSCode command palette:
   - F1 then ">Azurite: Start" to start the blob service locally (must be done before uploading image).
   - F1 then ">Azurite: Stop" to stop it when done.
   - F1 then ">Azurite: Clean" to clean up.
- Use Scalar endpoints to test blob upload functionality.
- Use Azure Storage Explorer (desktop app) to view/manage uploaded blobs.