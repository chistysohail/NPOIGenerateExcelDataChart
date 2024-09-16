NPOI Excel Report Generator - .NET 6
This project is a .NET 6 console application that generates an Excel file with sales data and a line chart using the NPOI library.

Features:
Creates an Excel file in .xlsx format.
Adds sales data for various products.
Generates a line chart based on product quantities.
Issue: No Fonts Found in Linux Container
When running the application inside a Linux-based Docker container, the following error occurred:


Unhandled exception. SixLabors.Fonts.FontException: No fonts found installed on the machine.
   at NPOI.SS.Util.SheetUtil.IFont2FontImpl(FontCacheKey cacheKey)
   at System.Collections.Concurrent.ConcurrentDictionary`2.GetOrAdd(TKey key, Func`2 valueFactory)
   at NPOI.SS.Util.SheetUtil.IFont2Font(IFont font1)
   at NPOI.SS.Util.SheetUtil.GetDefaultCharWidth(IWorkbook wb)
   at NPOI.SS.Util.SheetUtil.GetColumnWidth(ISheet sheet, Int32 column, Boolean useMergedCells, Int32 firstRow, Int32 lastRow, Int32 maxRows)
   at NPOI.SS.Util.SheetUtil.GetColumnWidth(ISheet sheet, Int32 column, Boolean useMergedCells)
   at NPOI.XSSF.UserModel.XSSFSheet.AutoSizeColumn(Int32 column, Boolean useMergedCells)
   at NPOI.XSSF.UserModel.XSSFSheet.AutoSizeColumn(Int32 column)
   at Program.Main(String[] args) in /src/Program.cs:line 42
   
Cause:
The error occurred because NPOI relies on system-installed fonts when performing operations like AutoSizeColumn. Linux-based Docker containers, by default, do not include fonts, which caused the error.

Solution: Install Font Dependencies in the Docker Container
To resolve this issue, the following font-related packages were installed in the Docker image:

libfontconfig1: A library that handles font configuration on Linux.
fonts-dejavu-core: A commonly used font family in Linux environments.
These dependencies ensure that fonts are available inside the container for NPOI to use when performing font-dependent tasks such as AutoSizeColumn.

Dockerfile
Here is the final Dockerfile used to resolve the issue:

Dockerfile
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
Steps to Build and Run the Application in Docker
1. Build the Docker Image
To build the Docker image, run the following command from the project root directory (where your Dockerfile is located):


docker build -t npoi-excel-report .
2. Run the Docker Container
Once the image is built, run the following command to execute the application inside the container:


docker run --rm -v $(pwd):/app/output npoi-excel-report
This command will run the application, and any generated files (e.g., the Excel report) will be saved to the local directory.

3. Expected Output
After running the container, an Excel file named SalesReportWithLineChart_NPOI.xlsx will be generated in your current directory. This file will contain both the sales data and the line chart.

Prerequisites
Docker: Ensure you have Docker installed and running on your machine.
.NET 6 SDK: If you wish to run the application locally without Docker.
