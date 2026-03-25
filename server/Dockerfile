# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first for layer caching
COPY TeamTasks.slnx ./
COPY server/TeamTasks.Api/TeamTasks.Api.csproj server/TeamTasks.Api/

RUN dotnet restore server/TeamTasks.Api/TeamTasks.Api.csproj

# Copy remaining source
COPY server/TeamTasks.Api/ server/TeamTasks.Api/

WORKDIR /src/server/TeamTasks.Api
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Create non-root user
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# Create data directory for SQLite (mounted as PVC in AKS)
RUN mkdir -p /data && chown appuser:appgroup /data

COPY --from=build /app/publish .
RUN chown -R appuser:appgroup /app

USER appuser

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "TeamTasks.Api.dll"]
