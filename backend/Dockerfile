FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/Asseta.Domain/*.csproj src/Asseta.Domain/
COPY src/Asseta.Application/*.csproj src/Asseta.Application/
COPY src/Asseta.Infrastructure/*.csproj src/Asseta.Infrastructure/
COPY src/Asseta.Api/*.csproj src/Asseta.Api/
RUN dotnet restore src/Asseta.Api/Asseta.Api.csproj

COPY . .
RUN dotnet publish src/Asseta.Api/Asseta.Api.csproj -c Release -o /out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "Asseta.Api.dll"]
