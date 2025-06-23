# ProductApi

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