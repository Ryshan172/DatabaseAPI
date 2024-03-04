namespace DatabaseApiCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ReviewerController : ControllerBase
    {
        private readonly string _connectionString;

        public ReviewerController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // Add a new reviewer to a StudentAllocation or UniversityApplication
        [HttpPost]
        public async Task<IActionResult> AddNewReviewer([FromBody] ReviewerModel reviewerModel)
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

                    var sql = "INSERT INTO Reviewers (UserID, StudentAllocationID, UniversityApplicationID) VALUES (@UserID, @StudentAllocationID, @UniversityApplicationID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", reviewerModel.UserID);
                        //command.Parameters.AddWithValue("@StudentAllocationID", reviewerModel.StudentAllocationID);

                        if (reviewerModel.StudentAllocationID.HasValue)
                        {
                            command.Parameters.AddWithValue("@StudentAllocationID", reviewerModel.StudentAllocationID.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@StudentAllocationID", DBNull.Value);
                        }

                        if (reviewerModel.UniversityApplicationID.HasValue)
                        {
                            command.Parameters.AddWithValue("@UniversityApplicationID", reviewerModel.UniversityApplicationID.Value);
                        }
                        else
                        {
                            command.Parameters.AddWithValue("@UniversityApplicationID", DBNull.Value);
                        }

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("User added successfully");
            }
            catch (Exception ex)
            {
                return NotFound("Could not add new reviwer");
            }
        }



        // Get Reviewers by different parameters 
        [HttpGet]
        public async Task<IActionResult> GetReviewers(int? userID, int? studentAllocationID, int? universityApplicationID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT * FROM Reviewers WHERE (@UserID IS NULL OR UserID = @UserID) AND (@StudentAllocationID IS NULL OR StudentAllocationID = @StudentAllocationID) AND (@UniversityApplicationID IS NULL OR UniversityApplicationID = @UniversityApplicationID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        // Add parameters only if they are not null
                        if (userID.HasValue)
                            command.Parameters.AddWithValue("@UserID", userID);
                        else
                            command.Parameters.AddWithValue("@UserID", DBNull.Value);

                        if (studentAllocationID.HasValue)
                            command.Parameters.AddWithValue("@StudentAllocationID", studentAllocationID);
                        else
                            command.Parameters.AddWithValue("@StudentAllocationID", DBNull.Value);

                        if (universityApplicationID.HasValue)
                            command.Parameters.AddWithValue("@UniversityApplicationID", universityApplicationID);
                        else
                            command.Parameters.AddWithValue("@UniversityApplicationID", DBNull.Value);

                        List<ReviewerModel> reviewers = new List<ReviewerModel>();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                ReviewerModel reviewer = new ReviewerModel
                                {
                                    // Populate reviewer model properties from reader
                                    UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                    StudentAllocationID = reader.IsDBNull(reader.GetOrdinal("StudentAllocationID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("StudentAllocationID")),
                                    UniversityApplicationID = reader.IsDBNull(reader.GetOrdinal("UniversityApplicationID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UniversityApplicationID"))
                            
                                };

                                reviewers.Add(reviewer);
                            }

                            // Replace null values with a unique identifier to represent "No application"
                            foreach (var reviewer in reviewers)
                            {
                                if (reviewer.StudentAllocationID == null)
                                {
                                    reviewer.StudentAllocationID = 0; // Unique identifier for "No application"
                                }
                                if (reviewer.UniversityApplicationID == null)
                                {
                                    reviewer.UniversityApplicationID = 0; 
                                }
                            }
                        }
                        return Ok(reviewers);
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("Could not get reviwers");
            }
        }



        
    }
}

