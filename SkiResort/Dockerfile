#FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
FROM ubuntu/dotnet-runtime:6.0-22.04_beta AS base
WORKDIR /app
EXPOSE 80

#FROM microsoft/dotnet:2.1-sdk AS build
#FROM bitnami/dotnet-sdk:latest AS build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY SkiResort.sln .
COPY BL/BL.csproj BL/
COPY BL.Tests/BL.Tests.csproj BL.Tests/
COPY AccessToDB/AccessToDB.csproj AccessToDB/
COPY AccessToDB.Tests/AccessToDB.Tests.csproj AccessToDB.Tests/
COPY E2ETests/E2ETests.csproj E2ETests/
COPY IntegrationTests/IntegrationTests.csproj IntegrationTests/
RUN dotnet restore

COPY . .
#WORKDIR /src/BL
#RUN dotnet build --no-restore -c Release -o /app

FROM build as test
WORKDIR /src/BL.Tests
RUN dotnet test

#FROM build AS publish
#WORKDIR /src/BL
#RUN dotnet publish --no-build -c Release -o /app

#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "BL.dll"]
