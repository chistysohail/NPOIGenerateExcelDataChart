# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Set the global NuGet package directory to avoid issues with missing folders
ENV NUGET_PACKAGES=/root/.nuget/packages

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore --force

# Copy the remaining files and build the app
COPY . ./
RUN dotnet build -c Release

# Publish the application
RUN dotnet publish -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS runtime
WORKDIR /app

# Install font dependencies to prevent "No fonts found" error
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       libfontconfig1 \
       fonts-dejavu-core \
    && rm -rf /var/lib/apt/lists/*

# Copy the build output from the previous stage
COPY --from=build /app/publish .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "NPOIGenerateExcelDataChart.dll"]
