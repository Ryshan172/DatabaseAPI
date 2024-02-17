using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using DatabaseApi.Models;

[Route("api/[controller]")]
[ApiController]
public class BursaryAllocationController : ControllerBase
{
    private readonly string _connectionString;

    public BursaryAllocationController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpPost]
    public async Task<IActionResult> AddBursaryAllocation([FromBody] BursaryAllocationModel bursaryallocation)
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
                    INSERT INTO BursaryAllocations (AmountAlloc, AllocationYear ,UniversityID)
                    VALUES (@AmountAlloc, @AllocationYear, @UniversityID)";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@AmountAlloc", bursaryallocation.AmountAllocated);
                    command.Parameters.AddWithValue("@AllocationYear", bursaryallocation.AllocatedYear);
                    command.Parameters.AddWithValue("@UniversityID",bursaryallocation.UniversityID);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok("Bursary amount Allocation added successfully");
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
}
