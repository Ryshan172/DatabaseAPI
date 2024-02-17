using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using DatabaseApi.Models;

namespace DatabaseApi.Controllers
{    // Controller for Inserting Values in the University Applications Table 
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
    }
}