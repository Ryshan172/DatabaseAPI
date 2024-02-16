using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Define connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register SqlConnection as a service
builder.Services.AddTransient(provider => new SqlConnection(connectionString));

// Register DataAccess as a service
builder.Services.AddTransient<DataAccess>(provider => new DataAccess(connectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Map a GET request to the "/roles" endpoint.
// When a GET request is made to "/roles", the specified callback function is executed.
app.MapGet("/roles", async (DataAccess dataAccess) =>
{
    // Use the DataAccess class to retrieve roles from the database asynchronously.
    var roles = await dataAccess.GetRolesAsync();

    // Return the retrieved roles as the response to the client.
    return roles;
})
// Assign a name to the route for documentation purposes e.g. "GetRoles"
.WithName("GetRoles")
// Enable OpenAPI (Swagger) documentation for this route. Allows route to be displayed in Swagger UI
.WithOpenApi();


// Get Request for Universities 
app.MapGet("/universities", async (DataAccess dataAccess) =>
{
    var universities = await dataAccess.GetUniversities();
    return universities;
})
.WithName("GetUniversities")
.WithOpenApi();


// Get Request for Departments 
app.MapGet("/departments", async (DataAccess dataAccess) =>
{
    var universities = await dataAccess.GetDepartments();
    return universities;
})
.WithName("GetDepartments")
.WithOpenApi();

// POST Request for Departments 
app.MapPost("/departments", async (DataAccess dataAccess, DepartmentModel departmentModel) =>
{
    try
    {
        // Access the department name from the departmentModel parameter
        //string departmentName = departmentModel.Department;
        string departmentName = "Eng";

        // Call AddDepartmentAsync from the DataAccess class to add the department to the database
        await dataAccess.AddDepartmentAsync(departmentName);

        // Return a successful response
        return new OkObjectResult("Department added successfully");
    }
    catch (Exception ex)
    {
        // Return an error response if an exception occurs
        var errorMessage = $"An error occurred: {ex.Message}";
        return new ObjectResult(errorMessage)
        {
            StatusCode = 500
        };
    }
})
// Assign a name to the route for documentation purposes e.g. "AddDepartment"
.WithName("AddDepartment")
// Enable OpenAPI (Swagger) documentation for this route. Allows route to be displayed in Swagger UI
.WithOpenApi();


app.Run();
