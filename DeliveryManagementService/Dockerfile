# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy only the project file to cache dependencies
COPY DeliveryManagementService.csproj ./
RUN dotnet restore DeliveryManagementService.csproj

# Copy all the application files
COPY . ./
RUN dotnet publish DeliveryManagementService.csproj -c Release -o /App/out

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

# Specify the entry point to run the application
ENTRYPOINT ["dotnet", "DeliveryManagementService.dll"]
