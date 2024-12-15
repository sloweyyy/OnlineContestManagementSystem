# Use a base image with .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use a separate image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OnlineContestManagement/OnlineContestManagement.csproj", "OnlineContestManagement/"]
RUN dotnet restore "OnlineContestManagement/OnlineContestManagement.csproj"
COPY . .
WORKDIR "/src/OnlineContestManagement"
RUN dotnet build "OnlineContestManagement.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "OnlineContestManagement.csproj" -c Release -o /app/publish
COPY OnlineContestManagement/Templates /app/publish/Templates

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OnlineContestManagement.dll"]
