using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using DatabaseApi.Models;

namespace DatabaseApi.Controllers
{    // Controller for Inserting Values in the Departments Table 
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly string _connectionString;
        public DepartmentsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] DepartmentModel departmentModel)

        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "INSERT INTO Departments (Department) VALUES (@Department)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Department", departmentModel.Department);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Department added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT Department FROM Departments";
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var departments = new List<string>();
                        while (await reader.ReadAsync())
                        {
                            departments.Add(reader.GetString(0));
                        }
                        return Ok(departments);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }

}
