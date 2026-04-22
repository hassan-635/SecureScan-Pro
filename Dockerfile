FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["FileIntegrityChecker/FileIntegrityChecker.csproj", "FileIntegrityChecker/"]
RUN dotnet restore "FileIntegrityChecker/FileIntegrityChecker.csproj"

# Copy everything else and build
COPY . .
WORKDIR /src/FileIntegrityChecker
RUN dotnet build "FileIntegrityChecker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FileIntegrityChecker.csproj" -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileIntegrityChecker.dll"]
