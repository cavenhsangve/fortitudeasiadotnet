# Use the official .NET Core SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the project files to the container
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the application files
COPY . ./

# Build the application
RUN dotnet publish -c Release -o out

# Use the official .NET Core runtime image to run the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set the working directory inside the runtime container
WORKDIR /app

# Copy the built application from the build image
COPY --from=build /app/out .

# Expose the application port
ENV ASPNETCORE_URLS=http://*:5001
EXPOSE 5001

# Define the entry point for the application
ENTRYPOINT ["dotnet", "api.dll"]
