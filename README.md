# Product Api

## Initial Setup

Please open visual studio developer command prompt and run the following commands:

```
dotnet tool install --global dotnet-ef --version 8.0.17
dotnet ef --version
```

Once done, then next run the migration commands. But before that please change the connection sting in appsettings.json

```
dotnet ef migrations add InitialEntities --project ProductApi.Infrastructure --startup-project ProductApi.Api
dotnet ef database update --project ProductApi.Infrastructure/ProductApi.Infrastructure.csproj --startup-project ProductApi.Api/ProductApi.Api.csproj
```

## Login credentials

When the project runs, for the first time it automatically seeds two users. Following are the user details:

```
Username: admin
Password: Dxfactor@123
UserRole: Admin

Username: user
Password: Dxfactor@123
UserRole: ProductUser
```


## Dependency flow

```
API → Application → Domain
API → Infrastructure → Domain
Application → Domain
Infrastructure → Domain
```

## Project Diagram

```
+-------------------+
|   Presentation    |  (ProductApi.Api)
+-------------------+
          |
          v
+-------------------+
|   Application     |  (ProductApi.Application)
+-------------------+
          |
          v
+-------------------+
|     Domain        |  (ProductApi.Domain)
+-------------------+
          ^
          |
+-------------------+
|  Infrastructure   |  (ProductApi.Infrastructure)
+-------------------+
```