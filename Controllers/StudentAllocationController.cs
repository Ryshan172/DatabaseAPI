
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
                        INSERT INTO StudentAllocations (Amount, AllocationYear, StudentIDNum, StudentMarks, CourseYear, ApplicationStatusID)
                        VALUES (@Amount, @AllocationYear, @StudentIDNum, @StudentMarks, @CourseYear, @ApplicationStatusID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Amount", StudentAllocationModel.Amount);
                        command.Parameters.AddWithValue("@AllocationYear", StudentAllocationModel.AllocationYear);
                        command.Parameters.AddWithValue("@StudentIDNum", StudentAllocationModel.StudentIDNum);
                        command.Parameters.AddWithValue("@StudentMarks", StudentAllocationModel.StudentMarks);
                        command.Parameters.AddWithValue("@CourseYear", StudentAllocationModel.CourseYear);
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

                    var sql = "SELECT AllocationID, Amount, AllocationYear, StudentIDNum, StudentMarks, CourseYear, ApplicationStatusID FROM StudentAllocations";
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
                                StudentIDNum = reader.GetString(3),
                                StudentMarks = reader.GetInt32(4),
                                CourseYear = reader.GetInt32(5),
                                ApplicationStatusID = reader.GetInt32(6)
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


        // Update Student Application By StudentID Number 
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudentAllocation(string id, [FromBody] StudentAllocationModel studentAllocationModel)
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
                        WHERE StudentIDNum = @StudentIDNum";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Amount", studentAllocationModel.Amount);
                        command.Parameters.AddWithValue("@AllocationYear", studentAllocationModel.AllocationYear);
                        command.Parameters.AddWithValue("@ApplicationStatusID", studentAllocationModel.ApplicationStatusID);
                        command.Parameters.AddWithValue("@AllocationID", id);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            return NotFound($"Student Allocation with StudentIDNum {id} not found");
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
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetStudentAllocationById(string studentId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT AllocationID, Amount, AllocationYear, StudentIDNum, StudentMarks, CourseYear, ApplicationStatusID FROM StudentAllocations WHERE StudentIDNum = @StudentIDNum";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StudentIDNum", studentId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var studentAllocation = new StudentAllocationModel
                                {
                                    AllocationID = reader.GetInt32(0),
                                    Amount = reader.GetDecimal(1),
                                    AllocationYear = reader.GetInt32(2),
                                    StudentIDNum = reader.GetString(3),
                                    StudentMarks = reader.GetInt32(4),
                                    CourseYear = reader.GetInt32(5),
                                    ApplicationStatusID = reader.GetInt32(6)
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


        // Search for Allocations By UniversityID 
        [HttpGet("university/{universityId}")]
        public async Task<IActionResult> GetStudentAllocationsByUniversityId(int universityId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"
                        SELECT SA.AllocationID, SA.Amount, SA.AllocationYear, SA.StudentIDNum, SA.StudentMarks, SA.CourseYear, SA.ApplicationStatusID
                        FROM StudentAllocations SA
                        INNER JOIN StudentsTable ST ON SA.StudentIDNum = ST.StudentIDNum
                        WHERE ST.UniversityID = @UniversityID";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UniversityID", universityId);

                        var studentAllocations = new List<StudentAllocationModel>();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var studentAllocation = new StudentAllocationModel
                                {
                                    AllocationID = reader.GetInt32(0),
                                    Amount = reader.GetDecimal(1),
                                    AllocationYear = reader.GetInt32(2),
                                    StudentIDNum = reader.GetString(3),
                                    StudentMarks = reader.GetInt32(4),
                                    CourseYear = reader.GetInt32(5),
                                    ApplicationStatusID = reader.GetInt32(6)
                                };
                                studentAllocations.Add(studentAllocation);
                            }
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



        [HttpPost("createStudentApplication")]
        public IActionResult CreateStudentApplication([FromBody] CreateStudentApplicationModel model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("EXEC [dbo].[CreateStudentApplication] @PhoneNumber, @Email, @FirstName, @LastName, @StudentIDNum, @DateOfBirth, @GenderID, @EthnicityID, @DepartmentID, @UniversityID, @Amount, @StudentMarks, @CourseYear", connection))
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
                        command.Parameters.AddWithValue("@LastName", model.LastName);
                        command.Parameters.AddWithValue("@StudentIDNum", model.StudentIDNum);
                        command.Parameters.AddWithValue("@DateOfBirth", model.DateOfBirth);
                        command.Parameters.AddWithValue("@GenderID", model.GenderID);
                        command.Parameters.AddWithValue("@EthnicityID", model.EthnicityID);
                        command.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                        command.Parameters.AddWithValue("@UniversityID", model.UniversityID);
                        command.Parameters.AddWithValue("@Amount", model.Amount);
                        command.Parameters.AddWithValue("@StudentMarks", model.StudentMarks);
                        command.Parameters.AddWithValue("@CourseYear", model.CourseYear);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("Student application created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }




    }
    
}
