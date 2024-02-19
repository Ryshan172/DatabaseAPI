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
                        INSERT INTO StudentsTable (StudentID, UserID, DateOfBirth, GenderID, EthnicityID, DepartmentID, UniversityID)
                        VALUES (@StudentID, @UserID, @DateOfBirth, @GenderID, @EthnicityID, @DepartmentID, @UniversityID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StudentID", studentModel.StudentID);
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
    }
}