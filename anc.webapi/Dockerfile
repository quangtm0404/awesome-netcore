#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["anc.webapi/anc.webapi.csproj", "anc.webapi/"]
COPY ["anc.domains/anc.domains.csproj", "anc.domains/"]
COPY ["anc.repositories/anc.repositories.csproj", "anc.repositories/"]
COPY ["anc.applications/anc.applications.csproj", "anc.applications/"]
RUN dotnet restore "./anc.webapi/anc.webapi.csproj"
COPY . .
WORKDIR "/src/anc.webapi"
RUN dotnet build "./anc.webapi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./anc.webapi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "anc.webapi.dll"]