
namespace DatabaseApiCode.Controllers
{    // Controller for Inserting Values in the University Applications Table 
    [Route("api/[controller]")]
    [ApiController]
    public class UniversityApplicationController : ControllerBase
    {
        private readonly string _connectionString;

        public UniversityApplicationController(IConfiguration configuration)

        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        // Main method for adding a university Application for funding from bbd 
        [HttpPost]
        public async Task<IActionResult> AddUniversityApplication([FromBody] UniversityApplicationModel universityApplicationModel)
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

                    // Check if an entry already exists for the UniversityID and Year
                    var checkSql = "SELECT COUNT(*) FROM UniversityApplication WHERE UniversityID = @UniversityID AND ApplicationYear = YEAR(GETDATE())";
                    using (var checkCommand = new SqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@UniversityID", universityApplicationModel.UniversityID);

                        int existingCount = (int)await checkCommand.ExecuteScalarAsync();
                        if (existingCount > 0)
                        {
                            // Entry already exists, return a conflict response
                            return Conflict("An entry for the same UniversityID for the current year already exists in the UniversityApplication table.");
                        }
                    }

                    // Entry doesn't exist, proceed with insertion
                    var sql = @"
                        INSERT INTO UniversityApplication (ApplicationStatusID, AmountRequested, UniversityID, ApplicationYear, IsLocked)
                        VALUES (1, @AmountRequested, @UniversityID, YEAR(GETDATE()), 1)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AmountRequested", universityApplicationModel.AmountRequested);
                        command.Parameters.AddWithValue("@UniversityID", universityApplicationModel.UniversityID);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("University Application added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Could not add University Application");
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

                    var sql = "SELECT ApplicationID, ApplicationStatusID, AmountRequested, UniversityID, ApplicationYear, IsLocked FROM UniversityApplication";
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var universityApplications = new List<UniversityApplicationModel>();
                        while (await reader.ReadAsync())
                        {
                            var universityApplication = new UniversityApplicationModel
                            {
                                ApplicationID = reader.GetInt32(0),
                                ApplicationStatusID = reader.GetInt32(1),
                                // Changed model to Decimal
                                AmountRequested = reader.GetDecimal(2),
                                UniversityID = reader.GetInt32(3),
                                ApplicationYear = reader.GetInt32(4), 
                                IsLocked = reader.GetBoolean(5) // Will this work because BIT on DB?
                            };
                            universityApplications.Add(universityApplication);
                        }
                        return Ok(universityApplications);
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("Could not find all University Applications");
            }
        }


        [HttpPut]
        public async Task<IActionResult> UpdateUniversityApplication([FromBody] UniversityApplicationModel universityApplicationModel)
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
                        UPDATE UniversityApplication 
                        SET ApplicationStatusID = @ApplicationStatusID,
                            AmountRequested = @AmountRequested,
                            UniversityID = @UniversityID,
                            ApplicationYear = @ApplicationYear,
                            IsLocked = @IsLocked
                        WHERE ApplicationID = @ApplicationID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationStatusID", universityApplicationModel.ApplicationStatusID);
                        command.Parameters.AddWithValue("@AmountRequested", universityApplicationModel.AmountRequested);
                        command.Parameters.AddWithValue("@UniversityID", universityApplicationModel.UniversityID);
                        command.Parameters.AddWithValue("@ApplicationYear", universityApplicationModel.ApplicationYear);
                        command.Parameters.AddWithValue("@IsLocked", universityApplicationModel.IsLocked);
                        command.Parameters.AddWithValue("@ApplicationID", universityApplicationModel.ApplicationID);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("University Application updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Could not add University Application");
            }
        }


    
        // Get Application by University Name
        [HttpGet("{universityName}")]
        public async Task<IActionResult> GetUniversityApplicationByName(string universityName)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Query to get the UniversityID based on the University Name
                    var getUniversityIdQuery = "SELECT UniversityID FROM Universities WHERE UniName = @UniversityName";
                    using (var getUniversityIdCommand = new SqlCommand(getUniversityIdQuery, connection))
                    {
                        getUniversityIdCommand.Parameters.AddWithValue("@UniversityName", universityName);
                        var universityId = await getUniversityIdCommand.ExecuteScalarAsync() as int?;

                        if (universityId.HasValue)
                        {
                            // Query to get the UniversityApplication based on the UniversityID
                            var sql = "SELECT ApplicationID, UniversityID, ApplicationStatusID, AmountRequested, ApplicationYear, IsLocked FROM UniversityApplication WHERE UniversityID = 1 AND ApplicationYear = YEAR(GETDATE())";
                            using (var command = new SqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@UniversityID", universityId.Value);

                                using (var reader = await command.ExecuteReaderAsync())
                                {
                                    if (await reader.ReadAsync())
                                    {
                                        var universityApplication = new UniversityApplicationModel
                                        {
                                            ApplicationID = reader.GetInt32(0),
                                            UniversityID = reader.GetInt32(1),
                                            ApplicationStatusID = reader.GetInt32(2),
                                            // Changed model to Decimal
                                            AmountRequested = reader.GetDecimal(3),
                                            ApplicationYear = reader.GetInt32(4),
                                            IsLocked = reader.GetBoolean(5) // Will this work because BIT on DB?
                                        };
                                        return Ok(universityApplication);
                                    }
                                    else
                                    {
                                        return NotFound("No university application for specified name"); // No university application found for the specified name
                                    }
                                }
                            }
                        }
                        else
                        {
                            return NotFound("No university application for specified name"); // University with the specified name not found
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("No university application for specified name");
            }
        }



        // Get all university applications along with their names
        [HttpGet("getAllUniversityApplicationsWithName")]
        public async Task<IActionResult> GetAllUniversityApplicationsWithName()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Query to get all university applications along with their names
                    var sql = @"
                        SELECT UA.ApplicationID, UA.UniversityID, U.UniName AS UniversityName, UA.ApplicationStatusID, UA.AmountRequested, UA.ApplicationYear, UA.IsLocked 
                        FROM UniversityApplication UA
                        INNER JOIN Universities U ON UA.UniversityID = U.UniversityID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        var universityApplications = new List<UniversityApplicationModelWithName>();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var universityApplication = new UniversityApplicationModelWithName
                                {
                                    ApplicationID = reader.GetInt32(0),
                                    UniversityID = reader.GetInt32(1),
                                    UniversityName = reader.GetString(2),
                                    ApplicationStatusID = reader.GetInt32(3),
                                    AmountRequested = reader.GetDecimal(4),
                                    ApplicationYear = reader.GetInt32(5),
                                    IsLocked = reader.GetBoolean(6)
                                };
                                universityApplications.Add(universityApplication);
                            }
                        }

                        return Ok(universityApplications);
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("Could not find all University Applications");
            }
        }



        // Delete University Application by ID
        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> DeleteUniversityApplicationById(int applicationId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the application exists
                    var checkSql = "SELECT COUNT(*) FROM UniversityApplication WHERE ApplicationID = @ApplicationID";
                    using (var checkCommand = new SqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@ApplicationID", applicationId);
                        var allocationCount = (int)await checkCommand.ExecuteScalarAsync();

                        if (allocationCount == 0)
                        {
                            return NotFound(); // Application with the specified ID not found
                        }
                    }

                    // Delete the allocation
                    var sql = "DELETE FROM UniversityApplication WHERE ApplicationID = @ApplicationID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@ApplicationID", applicationId);
                        await command.ExecuteNonQueryAsync();
                    }

                    return Ok("University Application deleted successfully");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Could not delete University Application");
            }
        }





    }
}
