# Use an official Node.js runtime as a base image
FROM bitnami/dotnet-sdk:latest

# Set the working directory in the container
WORKDIR /app


# Copy the application code to the container
COPY . .

# Install app dependencies
RUN dotnet restore "./DatabaseApiCode.csproj"

# Expose the port that the app will run on
EXPOSE 3000

# Define the command to run your application
CMD ["dotnet", "run"]
