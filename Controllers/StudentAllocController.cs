using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class StudentsAllocController : ControllerBase
{
    private readonly string _connectionString;

    public StudentsAllocController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent([FromBody] StudentAllocModel studentAllocModel)
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

                var sql = @"
                    INSERT INTO StudentAllocations (Amount, AllocationYear, StudentID)
                    VALUES (@Amount, @AllocationYear, @StudentID)";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Amount", studentAllocModel.Amount);
                    command.Parameters.AddWithValue("@AllocationYear", studentAllocModel.AllocationYear);
                    command.Parameters.AddWithValue("@StudentID", studentAllocModel.StudentID);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok("Student Allocation added successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

        /* 
        See if you can add catches and error messages for specifics. 
        E.g if User FK error then say "User does not exist"
        */
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAllStudentAllocations()
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = "SELECT Amount, AllocationYear, StudentID FROM StudentAllocations";
                using (var command = new SqlCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var studentAllocations = new List<StudentAllocModel>();
                    while (await reader.ReadAsync())
                    {
                        var studentAllocation = new StudentAllocModel
                        {   
                            // Needed to change the model to decimal
                            Amount = reader.GetDecimal(0),
                            AllocationYear = reader.GetInt32(1),
                            StudentID = reader.GetInt32(2)
                        };
                        studentAllocations.Add(studentAllocation);
                    }
                    return Ok(studentAllocations);
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

}
