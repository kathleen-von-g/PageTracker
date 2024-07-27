ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:8.0
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:8.0

FROM ${DOTNET_RUNTIME} AS base
ENV ASPNETCORE_URLS=http://+:7105
WORKDIR /app
EXPOSE 7105

FROM ${DOTNET_SDK} AS build
WORKDIR /src
COPY ["src/PageTracker.Api/PageTracker.Api.csproj", "src/PageTracker.Api/"]
COPY ["src/PageTracker.Domain/PageTracker.Domain.csproj", "src/PageTracker.Domain/"]
COPY ["src/PageTracker.Common/PageTracker.Common.csproj", "src/PageTracker.Common/"]
COPY ["src/PageTracker.Infrastructure/PageTracker.Infrastructure.csproj", "src/PageTracker.Infrastructure/"]
RUN dotnet restore "./src/PageTracker.Api/PageTracker.Api.csproj"

COPY ["src/", "src/"]

## Run migrations
FROM build as migrations
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT dotnet-ef database update --project src/PageTracker.Infrastructure/ --startup-project src/PageTracker.Api/