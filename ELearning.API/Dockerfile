# Runtime image for the API container.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

USER root
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*
USER $APP_UID

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl -fsS http://localhost:8080/health/live || exit 1

# Build the API project from the solution source.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ELearning.API/ELearning.API.csproj", "ELearning.API/"]
COPY ["ELearning.Application/ELearning.Application.csproj", "ELearning.Application/"]
COPY ["ELearning.Domain/ELearning.Domain.csproj", "ELearning.Domain/"]
COPY ["ELearning.SharedKernel/ELearning.SharedKernel.csproj", "ELearning.SharedKernel/"]
COPY ["ELearning.Infrastructure/ELearning.Infrastructure.csproj", "ELearning.Infrastructure/"]
RUN dotnet restore "./ELearning.API/ELearning.API.csproj"
COPY . .
WORKDIR "/src/ELearning.API"
RUN dotnet build "./ELearning.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the compiled API.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ELearning.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image.
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ELearning.API.dll"]
