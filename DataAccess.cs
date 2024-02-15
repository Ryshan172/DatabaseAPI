using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;


/// Provides data access methods for interacting with the database.
public class DataAccess
{
    private readonly string _connectionString;

    /*
     Initializes a new instance of the <see cref="DataAccess"/> class with the specified connection string.
    <param name="connectionString">The connection string to the database. Found in appsettings.json</param>
    */
    public DataAccess(string connectionString)
    {
        _connectionString = connectionString;
    }

    /*
    Retrieves a list of roles asynchronously from the database.
    Returns a task that represents the asynchronous operation. 
    The task result contains the list of roles.
    */ 
    public async Task<List<string>> GetRolesAsync()
    {
        // Establish a connection to the database
        using (var connection = new SqlConnection(_connectionString))
        {
            // Open the database connection asynchronously
            await connection.OpenAsync();

            // Initialize a list to store the roles
            var roles = new List<string>();

            // Create a SQL command to select role names from the Roles table
            using (var command = new SqlCommand("SELECT RoleName FROM Roles", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                // Read data from the database asynchronously
                while (await reader.ReadAsync())
                {
                    // Retrieve the role name from the current row and add it to the list
                    roles.Add(reader.GetString(0));
                }
            }

            // Return the list of roles retrieved from the database
            return roles;
        }
    }

    // Need to add more methods to retrieve other data 

    // GET Method for Universities Table
    public async Task<List<string>> GetUniversities()
    {
        // Establish a connection to the database
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Initialize a list to store the roles
            var universities = new List<string>();

            // SQL to Select University Names 
            using (var command = new SqlCommand("SELECT UniName FROM Universities ", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    universities.Add(reader.GetString(0));
                }
            }

            return universities;
        }
    }


    // GET Method for Departments Table
    public async Task<List<string>> GetDepartments()
    {
        // Establish a connection to the database
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();

            // Initialize a list to store the roles
            var departments = new List<string>();

            // SQL to Select Department Names 
            using (var command = new SqlCommand("SELECT Department FROM Departments ", connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    departments.Add(reader.GetString(0));
                }
            }

            return departments;
        }
    }

    public async Task AddDepartmentAsync(string departmentName)
    {
        
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = "INSERT INTO Departments (Department) VALUES (@Department)";
                using (var command = new SqlCommand(sql, connection))
                {   
                    
                    command.Parameters.AddWithValue("@Department", departmentName);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            // Handle exception as needed
            throw new Exception($"Error adding department: {ex.Message}");
        }
    }

    
    


        
}
