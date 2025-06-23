# ProductApi

```
dotnet tool install --global dotnet-ef --version 8.0.17
dotnet ef --version
```

```
dotnet ef migrations add InitialEntities --project ProductApi.Infrastructure --startup-project ProductApi.Api
dotnet ef database update --project ProductApi.Infrastructure/ProductApi.Infrastructure.csproj --startup-project ProductApi.Api/ProductApi.Api.csproj
```