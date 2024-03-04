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
        [HttpPut]
        public async Task<IActionResult> UpdateStudentAllocation([FromBody] FullStudentAppModel studentAllocationModel)
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
                            ApplicationStatusID = @ApplicationStatusID, 
                            StudentMarks = @StudentMarks,
                            CourseYear = @CourseYear
                        WHERE AllocationID = @AllocationID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Amount", studentAllocationModel.Amount);
                        command.Parameters.AddWithValue("@AllocationYear", studentAllocationModel.AllocationYear);
                        command.Parameters.AddWithValue("@ApplicationStatusID", studentAllocationModel.ApplicationStatusID);
                        command.Parameters.AddWithValue("@StudentMarks", studentAllocationModel.StudentMarks);
                        command.Parameters.AddWithValue("@CourseYear", studentAllocationModel.CourseYear);
                        command.Parameters.AddWithValue("@StudentIDNum", studentAllocationModel.StudentIDNum);
                        command.Parameters.AddWithValue("@AllocationID", studentAllocationModel.AllocationID);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            return NotFound($"Student Allocation with Specified StudentIDNum not found");
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


        [HttpPut("ApplicationStatus")]
        public async Task<IActionResult> UpdateStudentAllocStatus(string id, [FromBody] UpdateStudentAllocationModel updateStudentAllocationModel)
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
                        SET ApplicationStatusID = @ApplicationStatusID 
                        WHERE StudentIDNum = @StudentIDNum";
                    using (var command = new SqlCommand(sql, connection))
                    {   
                        command.Parameters.AddWithValue("@StudentIDNum", updateStudentAllocationModel.StudentIDNum);
                        command.Parameters.AddWithValue("@ApplicationStatusID", updateStudentAllocationModel.ApplicationStatusID);

                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected == 0)
                        {
                            return NotFound($"Student Allocation with specified StudentIDNum not found");
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


        [HttpGet("GetStudentAllocationByIdPro/{IdNumber}")]
        public async Task<IActionResult> GetStudentAllocationByIdPro(string IdNumber)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"SELECT 
                        StudentAllocations.Amount,
                        StudentAllocations.AllocationYear AS ApplicationYear,
                        StudentAllocations.StudentMarks,
                        StudentAllocations.CourseYear,
                        StudentAllocations.ApplicationStatusID,
                        StudentAllocations.AllocationID,	
                        Users.FirstName,
                        Users.LastName,
                        ContactDetails.Email,
                        ContactDetails.PhoneNumber,
                        StudentsTable.StudentIDNum,
                        StudentsTable.DateOfBirth,
                        Departments.Department,
                        Ethnicity.Ethnicity,
                        Genders.Gender,
                        Universities.UniName AS University,
                        Universities.UniversityID AS UniversityID
                        FROM StudentsTable
                        JOIN StudentAllocations
                        ON StudentsTable.StudentIDNum = StudentAllocations.StudentIDNum
                        JOIN Users
                        ON StudentsTable.UserID = Users.UserID
                        JOIN ContactDetails
                        ON ContactDetails.UserID = Users.UserID
                        JOIN Ethnicity
                        ON StudentsTable.EthnicityID = Ethnicity.EthnicityID
                        JOIN Departments
                        ON StudentsTable.DepartmentID = Departments.DepartmentID
                        JOIN Genders
                        ON StudentsTable.GenderID = Genders.GenderID
                        JOIN Universities
                        ON StudentsTable.UniversityID = Universities.UniversityID
                        WHERE StudentsTable.StudentIDNum = @StudentIDNum";
                    SqlCommand command = new SqlCommand(sql, connection);
                    
                    command.Parameters.AddWithValue("@StudentIDNum", IdNumber);

                    var reader = await command.ExecuteReaderAsync();
                        
                            if (await reader.ReadAsync())
                            {
                                
                                return Ok(new ProStudentModel{
                                    Amount = reader.GetDecimal(0),
                                    AllocationYear = reader.GetInt32(1),
                                    StudentMarks = reader.GetInt32(2),
                                    CourseYear = reader.GetInt32(3),
                                    ApplicationStatusID = reader.GetInt32(4),
                                    AllocationID = reader.GetInt32(5),
                                    FirstName = reader.GetString(6),
                                    LastName = reader.GetString(7),
                                    Email = reader.GetString(8),
                                    PhoneNumber = reader.GetString(9),
                                    StudentIdNumber = reader.GetString(10),
                                    DateOfBirth = reader.GetDateTime(11),
                                    Department = reader.GetString(12),
                                    Ethnicity = reader.GetString(13),
                                    Gender = reader.GetString(14),
                                    University = reader.GetString(15),
                                    UniversityID = reader.GetInt32(16)
                                }); 
                        
                            }
                            else
                            {
                                return NotFound(); // Allocation with the specified ID not found
                            }
                        
                    }
                
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        [HttpGet("GetAllStudentAllocationsPro")]
        public async Task<IActionResult> GetAllStudentAllocationsPro()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    string sql = @"SELECT 
                        StudentAllocations.Amount,
                        StudentAllocations.AllocationYear AS ApplicationYear,
                        StudentAllocations.StudentMarks,
                        StudentAllocations.CourseYear,
                        StudentAllocations.ApplicationStatusID,
                        StudentAllocations.AllocationID,	
                        Users.FirstName,
                        Users.LastName,
                        ContactDetails.Email,
                        ContactDetails.PhoneNumber,
                        StudentsTable.StudentIDNum,
                        StudentsTable.DateOfBirth,
                        Departments.Department,
                        Ethnicity.Ethnicity,
                        Genders.Gender,
                        Universities.UniName AS University,
                        Universities.UniversityID AS UniversityID
                        FROM StudentsTable
                        JOIN StudentAllocations
                        ON StudentsTable.StudentIDNum = StudentAllocations.StudentIDNum
                        JOIN Users
                        ON StudentsTable.UserID = Users.UserID
                        JOIN ContactDetails
                        ON ContactDetails.UserID = Users.UserID
                        JOIN Ethnicity
                        ON StudentsTable.EthnicityID = Ethnicity.EthnicityID
                        JOIN Departments
                        ON StudentsTable.DepartmentID = Departments.DepartmentID
                        JOIN Genders
                        ON StudentsTable.GenderID = Genders.GenderID
                        JOIN Universities
                        ON StudentsTable.UniversityID = Universities.UniversityID";
                    SqlCommand command = new SqlCommand(sql, connection);
                    var reader = await command.ExecuteReaderAsync();

                        List<ProStudentModel> studentAllocations = new List<ProStudentModel>();
                        while (await reader.ReadAsync())
                        {
                           ProStudentModel studentAllocation = new ProStudentModel
                            {
                                Amount = reader.GetDecimal(0),
                                AllocationYear = reader.GetInt32(1),
                                StudentMarks = reader.GetInt32(2),
                                CourseYear = reader.GetInt32(3),
                                ApplicationStatusID = reader.GetInt32(4),
                                AllocationID = reader.GetInt32(5),
                                FirstName = reader.GetString(6).Trim(),
                                LastName = reader.GetString(7).Trim(),
                                Email = reader.GetString(8).Trim(),
                                PhoneNumber = reader.GetString(9),
                                StudentIdNumber = reader.GetString(10).Trim(),
                                DateOfBirth = reader.GetDateTime(11),
                                Department = reader.GetString(12),
                                Ethnicity = reader.GetString(13),
                                Gender = reader.GetString(14),
                                University = reader.GetString(15),
                               UniversityID = reader.GetInt32(16)

                           };
                            studentAllocations.Add(studentAllocation);
                        }
                        return Ok(studentAllocations);
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


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetHODStudentAppsByUserId(int userId)
        {
            try
            {
                // Retrieve the UniversityID associated with the UserID
                int universityId = 0; // Initialize universityId variable

                // Connect to the database
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Query to retrieve UniversityID and UniName from UniversityUser and Universities tables
                    var universityQuery = @"
                        SELECT UU.UniversityID, U.UniName
                        FROM UniversityUser UU
                        INNER JOIN Universities U ON UU.UniversityID = U.UniversityID
                        WHERE UU.UserID = @UserID";

                    // Execute the query
                    using (var universityCommand = new SqlCommand(universityQuery, connection))
                    {
                        universityCommand.Parameters.AddWithValue("@UserID", userId);
                        var universityReader = await universityCommand.ExecuteReaderAsync();

                        // Check if UniversityID exists for the given UserID
                        if (universityReader.Read())
                        {
                            universityId = universityReader.GetInt32(0);
                        }
                        else
                        {
                            return NotFound("University not found for the given user.");
                        }
                    }
                }

                // Query to retrieve student allocations by UniversityID
                var sql = @"
                    SELECT SA.AllocationID, SA.Amount, SA.AllocationYear, ST.StudentIDNum, SA.StudentMarks, SA.CourseYear, SA.ApplicationStatusID, U.UniName, 
                    U2.FirstName AS StudentFirstName, U2.LastName AS StudentLastName
                    FROM StudentAllocations SA
                    INNER JOIN StudentsTable ST ON SA.StudentIDNum = ST.StudentIDNum
                    INNER JOIN Users U2 ON ST.UserID = U2.UserID  -- Join with Users table to get student's first name and last name
                    INNER JOIN Universities U ON ST.UniversityID = U.UniversityID
                    WHERE ST.UniversityID = @UniversityID;";

                // Connect to the database and execute the query
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UniversityID", universityId);

                        var studentAllocations = new List<HODStudentAllocationModel>();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var hodstudentAllocation = new HODStudentAllocationModel
                                {
                                    AllocationID = reader.GetInt32(0),
                                    Amount = reader.GetDecimal(1),
                                    AllocationYear = reader.GetInt32(2),
                                    StudentIDNum = reader.GetString(3),
                                    StudentMarks = reader.GetInt32(4),
                                    CourseYear = reader.GetInt32(5),
                                    ApplicationStatusID = reader.GetInt32(6),
                                    UniName = reader.GetString(7), // Retrieve UniName from the query result
                                    StudentFirstName = reader.GetString(8).Trim(),
                                    StudentLastName = reader.GetString(9).Trim(),
                                };
                                studentAllocations.Add(hodstudentAllocation);
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



        // Delete Student Allocation by ID
        [HttpDelete("{allocationId}")]
        public async Task<IActionResult> DeleteStudentAllocationById(int allocationId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    // Check if the allocation exists
                    var checkSql = "SELECT COUNT(*) FROM StudentAllocations WHERE AllocationID = @AllocationID";
                    using (var checkCommand = new SqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@AllocationID", allocationId);
                        var allocationCount = (int)await checkCommand.ExecuteScalarAsync();

                        if (allocationCount == 0)
                        {
                            return NotFound(); // Allocation with the specified ID not found
                        }
                    }

                    // Delete the allocation
                    var sql = "DELETE FROM StudentAllocations WHERE AllocationID = @AllocationID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AllocationID", allocationId);
                        await command.ExecuteNonQueryAsync();
                    }

                    return Ok("Student allocation deleted successfully");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }





    }
    
}
