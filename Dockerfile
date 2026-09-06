# ==========================================
# Stage 1: Build React Frontend (Airbnb Style)
# ==========================================
FROM node:20-alpine AS frontend-build
WORKDIR /frontend

# Copy package files and install dependencies
COPY frontend-web/package*.json ./
RUN npm install

# Copy source and build production bundle
COPY frontend-web/ ./
RUN npm run build

# ==========================================
# Stage 2: Build .NET 8 Backend API
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build
WORKDIR /backend

# Copy csproj files to leverage Docker layer caching
COPY backend/src/Asseta.Domain/*.csproj src/Asseta.Domain/
COPY backend/src/Asseta.Application/*.csproj src/Asseta.Application/
COPY backend/src/Asseta.Infrastructure/*.csproj src/Asseta.Infrastructure/
COPY backend/src/Asseta.Api/*.csproj src/Asseta.Api/

# Restore only the API and its project dependencies (avoids missing test project errors)
RUN dotnet restore src/Asseta.Api/Asseta.Api.csproj

# Copy entire backend source and publish
COPY backend/ .
RUN dotnet publish src/Asseta.Api/Asseta.Api.csproj -c Release -o /out --no-restore

# ==========================================
# Stage 3: Unified Production Runtime Image
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy .NET published binaries
COPY --from=backend-build /out .

# Copy React Frontend dist into wwwroot served directly by ASP.NET Core
COPY --from=frontend-build /frontend/dist ./wwwroot

# Expose standard web container port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Asseta.Api.dll"]
