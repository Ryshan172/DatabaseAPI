#pragma warning disable CS8601
#pragma warning disable CS1591
#pragma warning disable CS8618

namespace DatabaseApiCode.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsAllocationController : ControllerBase
    {
        private readonly string _connectionString;

        public StudentsAllocationController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] StudentAllocationModel StudentAllocationModel)
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
                        INSERT INTO StudentAllocations (Amount, AllocationYear, StudentID, ApplicationStatusID)
                        VALUES (@Amount, @AllocationYear, @StudentID, @ApplicationStatusID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Amount", StudentAllocationModel.Amount);
                        command.Parameters.AddWithValue("@AllocationYear", StudentAllocationModel.AllocationYear);
                        command.Parameters.AddWithValue("@StudentID", StudentAllocationModel.StudentID);
                        command.Parameters.AddWithValue("@ApplicationStatusID", StudentAllocationModel.ApplicationStatusID);
                        
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

                    var sql = "SELECT AllocationID, Amount, AllocationYear, StudentID, ApplicationStatusID FROM StudentAllocations";
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var studentAllocations = new List<StudentAllocationModel>();
                        while (await reader.ReadAsync())
                        {
                            var studentAllocation = new StudentAllocationModel
                            {   
                                // Needed to change the model to decimal
                                AllocationID = reader.GetInt32(0),
                                Amount = reader.GetDecimal(1),
                                AllocationYear = reader.GetInt32(2),
                                StudentID = reader.GetInt32(3),
                                ApplicationStatusID = reader.GetInt32(4)
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


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudentAllocation(int id, [FromBody] StudentAllocationModel studentAllocationModel)
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
                        UPDATE StudentAllocations 
                        SET Amount = @Amount, 
                            AllocationYear = @AllocationYear, 
                            ApplicationStatusID = @ApplicationStatusID 
                        WHERE AllocationID = @AllocationID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Amount", studentAllocationModel.Amount);
                        command.Parameters.AddWithValue("@AllocationYear", studentAllocationModel.AllocationYear);
                        command.Parameters.AddWithValue("@ApplicationStatusID", studentAllocationModel.ApplicationStatusID);
                        command.Parameters.AddWithValue("@AllocationID", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            return NotFound($"Student Allocation with AllocationID {id} not found");
                        }
                    }
                }

                return Ok("Student Allocation updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        // Search for Student Allocation by ID
        [HttpGet("{allocationId}")]
        public async Task<IActionResult> GetStudentAllocationById(int allocationId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT AllocationID, Amount, AllocationYear, StudentID, ApplicationStatusID FROM StudentAllocations WHERE AllocationID = @AllocationID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AllocationID", allocationId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var studentAllocation = new StudentAllocationModel
                                {
                                    AllocationID = reader.GetInt32(0),
                                    Amount = reader.GetDecimal(1),
                                    AllocationYear = reader.GetInt32(2),
                                    StudentID = reader.GetInt32(3),
                                    ApplicationStatusID = reader.GetInt32(4)
                                };
                                return Ok(studentAllocation);
                            }
                            else
                            {
                                return NotFound(); // Allocation with the specified ID not found
                            }
                        }
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
