# Backend Instructions
The backend server is built using ASP.NET Core Minimal APIs and Entity Framework Core. It supports product management, image upload to blob storage, and an AI-powered image analysis endpoint.

## Running the Server
1. Navigate to the `\reko-mini-project.Server\` directory in your terminal.
2. Run:
```
   dotnet run
```
3. The server starts on the configured URL. The default development URL is:
```
   https://localhost:7213
```

## Running Tests
The backend has a dedicated test project at `reko-mini-project.Tests\reko-mini-project.Tests.csproj`. It includes xUnit tests for product logic, image validation, image upload behavior, and multipart endpoint handling.

From the solution root:
```
   dotnet test reko-market-mvp.slnx
```

From the server folder:
```
   dotnet test ..\reko-mini-project.Tests\reko-mini-project.Tests.csproj
```

In VS Code:
- Open the Testing view
- Find `reko-mini-project.Tests`
- Run individual test files or the full test project

The test project uses:
- `xunit`
- `Microsoft.EntityFrameworkCore.InMemory` for database-backed unit tests
- `Microsoft.AspNetCore.Mvc.Testing` for API endpoint integration tests

## Configuration
The server uses `appsettings.json` for shared defaults and `appsettings.Development.json` for local development overrides.
- `appsettings.Development.json` is intentionally excluded from source control to protect local configuration.
- Use `appsettings.Development.example.json` as the template for local development settings.
- Do not commit secrets, connection strings, or personal environment values.
- For sensitive values, prefer `dotnet user-secrets` or environment variables.

## Active API Endpoints
The server currently exposes these endpoints:
1. `POST /api/products`
   - Create a new product with a multipart/form-data request and an uploaded image.
2. `POST /api/products/analyze`
   - Analyze an uploaded image using AI and return extracted product suggestions.
3. `PUT /api/products/{id}`
   - Update an existing product by its GUID identifier using JSON body data.
4. `PUT /api/products/{id}/with-image`
   - Update a product and replace its image using multipart/form-data.
5. `GET /api/products`
   - Retrieve all products.
6. `GET /api/products/{id}`
   - Retrieve a specific product by its GUID identifier.
7. `DELETE /api/products/{id}`
   - Delete a product by its GUID identifier.

> Note: The server no longer exposes a separate `/api/images/upload` endpoint. Image uploads are handled through the product create/update endpoints.

### Scalar Testing
Scalar is enabled in development. Open:
```
   https://localhost:7213/Scalar
```
Use the UI to select an endpoint, build requests, and send test data.

## CORS Configuration
The server allows cross-origin requests from the frontend origin defined in `Properties/launchSettings.json` and `appsettings.Development.json`.
- Default development origin: `http://localhost:5173`
- Configure `ALLOWED_ORIGINS` in launch settings or environment variables for other hosts.

## Database Configuration
The server uses Entity Framework Core with SQLite for development storage.
- Connection string is defined in `appsettings.Development.json` under `ConnectionStrings:DefaultConnection`.
- Default SQLite file: `Data/products.db`.

### Reset database
To reset local data:
1. Delete `Data/products.db`
2. Remove files under `Data/Migrations`

### Recreate database
Run:
```
   dotnet ef migrations add InitialCreate --output-dir .\Data\Migrations\
```
Then:
```
   dotnet ef database update
```

## Image Upload and Blob Storage
Image handling uses Azure Blob Storage via the application configuration.
- If `BlobStorage:ConnectionString` is present, the app uses `BlobServiceClient(connectionString)`.
- Otherwise it uses `BlobServiceClient(serviceUri, DefaultAzureCredential)` with `BlobStorage:ServiceUri`.
- `BlobStorage:ContainerName` is required in all environments.

### Development
- Local development configuration should be based on `appsettings.Development.example.json` and kept out of Git.
- The example file includes the local blob storage container name and a safe Azurite connection string placeholder.
- `BlobStorage:ContainerName` is required and can remain safely documented in the repo.
- Use Azurite for local blob storage emulation.

### Production
For Azure-hosted deployments, the recommended setup is:
1. Azure Storage Account with Blob service
2. Blob container
3. Managed Identity on the app service or container
4. `Storage Blob Data Contributor` RBAC for the identity

Recommended app settings:
- `BlobStorage__ServiceUri=https://<storage-account>.blob.core.windows.net`
- `BlobStorage__ContainerName=<container-name>`

> Do not set `BlobStorage__ConnectionString` in production if you intend to use Managed Identity.

### Testing with Azurite, Scalar, and Storage Explorer
- Use the Azurite VS Code extension for local blob storage emulation.
  - Start the local blob service before running image upload scenarios.
  - Stop Azurite when done to free the port.
  - Clean local storage when you want a fresh blob container state.
- Local development configuration should be based on `appsettings.Development.example.json` and kept out of Git.
  - The example file includes the local blob storage container name and a safe Azurite connection string placeholder.
  - `BlobStorage:ContainerName` is required and can remain safely documented in the repo.
- The server stores uploaded files to blob storage when handling product create/update requests.
- Scalar is available in development at:
```
   https://localhost:7213/Scalar
```
  - Use Scalar to test individual API endpoints and inspect request/response payloads.
  - For multipart form uploads, select the endpoint, attach the file, then send the request.
- Use Azure Storage Explorer to inspect blob container contents when using Azurite or Azure Storage.
  - When testing locally with Azurite, connect using the local emulator endpoint (usually `http://127.0.0.1:10000/devstoreaccount1`).
  - Confirm that uploaded image blobs are created in the configured blob container.

### Notes
- Product creation and update operations upload images directly as part of the product request.
- The image analysis endpoint returns `503`/`500` or `422` when AI analysis is unavailable or fails.
- In development, sample product data is seeded automatically when the app runs under `Development` environment.
