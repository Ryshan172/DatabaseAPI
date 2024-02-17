using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

// Controller for Inserting Values in the University Applications Table 
[Route("api/[controller]")]
[ApiController]
public class UniAppController : ControllerBase
{
    private readonly string _connectionString;

    public UniAppController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpPost]
    public async Task<IActionResult> AddDepartment([FromBody] UniApplicationModel uniApplicationModel)
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
                    INSERT INTO UniversityApplication (ApplicationStatusID, AmountRequested, UniversityID)
                    VALUES (@ApplicationStatusID, @AmountRequested, @UniversityID)";
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ApplicationStatusID", uniApplicationModel.ApplicationStatusID);
                    command.Parameters.AddWithValue("@AmountRequested", uniApplicationModel.AmountRequested);
                    command.Parameters.AddWithValue("@UniversityID", uniApplicationModel.UniversityID);
                    
                    await command.ExecuteNonQueryAsync();
                }
            }

            return Ok("University Application added successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }

    }

    [HttpGet]
    public async Task<IActionResult> GetAllUniversityApplications()
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var sql = "SELECT ApplicationStatusID, AmountRequested, UniversityID FROM UniversityApplication";
                using (var command = new SqlCommand(sql, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var universityApplications = new List<UniApplicationModel>();
                    while (await reader.ReadAsync())
                    {
                        var uniApplication = new UniApplicationModel
                        {
                            ApplicationStatusID = reader.GetInt32(0),
                            // Changed model to Decimal
                            AmountRequested = reader.GetDecimal(1),
                            UniversityID = reader.GetInt32(2)
                        };
                        universityApplications.Add(uniApplication);
                    }
                    return Ok(universityApplications);
                }
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }

}
