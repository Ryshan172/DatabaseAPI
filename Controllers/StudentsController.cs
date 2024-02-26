
namespace DatabaseApiCode.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly string _connectionString;


        public StudentsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost]
        public async Task<IActionResult> AddStudent([FromBody] StudentModel studentModel)
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
                        INSERT INTO StudentsTable (StudentIDNum, UserID, DateOfBirth, GenderID, EthnicityID, DepartmentID, UniversityID)
                        VALUES (@StudentIDNum, @UserID, @DateOfBirth, @GenderID, @EthnicityID, @DepartmentID, @UniversityID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StudentIDNum", studentModel.StudentIDNum);
                        command.Parameters.AddWithValue("@UserID", studentModel.UserID);
                        command.Parameters.AddWithValue("@DateOfBirth", studentModel.DateOfBirth);
                        command.Parameters.AddWithValue("@GenderID", studentModel.GenderID);
                        command.Parameters.AddWithValue("@EthnicityID", studentModel.EthnicityID);
                        command.Parameters.AddWithValue("@DepartmentID", studentModel.DepartmentID);
                        command.Parameters.AddWithValue("@UniversityID", studentModel.UniversityID);

                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Student added successfully");
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


        [HttpGet("students")]
        public async Task<IActionResult> GetAllStudents()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"
                        SELECT S.StudentIDNum, U.FirstName, U.LastName, U.RoleID, U.UserID, S.DateOfBirth, S.GenderID, S.EthnicityID, S.DepartmentID, S.UniversityID
                        FROM StudentsTable S
                        INNER JOIN Users U ON S.UserID = U.UserID";

                    var students = new List<StudentModel>();

                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var student = new StudentModel
                            {
                                StudentIDNum = reader.GetString(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                RoleID = reader.GetInt32(3),
                                UserID = reader.GetInt32(4),
                                DateOfBirth = reader.GetDateTime(5),
                                GenderID = reader.GetInt32(6),
                                EthnicityID = reader.GetInt32(7),
                                DepartmentID = reader.GetInt32(8),
                                UniversityID = reader.GetInt32(9),
                            
                            };
                            students.Add(student);
                        }
                    }

                    return Ok(students);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        // Search for Students By ID Number: 
        [HttpGet("students/{studentIDNum}")]
        public async Task<IActionResult> GetStudentByStudentIDNum(string studentIDNum)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = @"
                        SELECT S.StudentIDNum, U.FirstName, U.LastName, U.RoleID, U.UserID, S.DateOfBirth, S.GenderID, S.EthnicityID, S.DepartmentID, S.UniversityID
                        FROM StudentsTable S
                        INNER JOIN Users U ON S.UserID = U.UserID
                        WHERE S.StudentIDNum = @StudentIDNum";

                    var students = new List<StudentModel>();

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StudentIDNum", studentIDNum);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var student = new StudentModel
                                {
                                    StudentIDNum = reader.GetString(0),
                                    FirstName = reader.GetString(1),
                                    LastName = reader.GetString(2),
                                    RoleID = reader.GetInt32(3),
                                    UserID = reader.GetInt32(4),
                                    DateOfBirth = reader.GetDateTime(5),
                                    GenderID = reader.GetInt32(6),
                                    EthnicityID = reader.GetInt32(7),
                                    DepartmentID = reader.GetInt32(8),
                                    UniversityID = reader.GetInt32(9),
                                };
                                students.Add(student);
                            }
                        }
                    }

                    return Ok(students);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}