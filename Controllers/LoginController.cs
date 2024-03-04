namespace DatabaseApiCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class LoginController : ControllerBase
    {
        private readonly string _connectionString;

        public LoginController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("byEmail")]
        [ProducesResponseType(typeof(UserModel), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetUserDetailsByEmail([FromQuery] string userEmail)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = @"
                    SELECT u.UserID, u.RoleID, u.FirstName, u.LastName
                    FROM Users u
                    WHERE u.UserID = (
                        SELECT UserID
                        FROM ContactDetails
                        WHERE Email = @UserEmail
                    )";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserEmail", userEmail);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        UserModel userDetails = new UserModel
                        {   
                            UserID = reader.GetInt32(0),
                            RoleID = reader.GetInt32(1),
                            FirstName = reader.GetString(2),
                            LastName = reader.GetString(3)
                        };

                        reader.Close();
                        return Ok(userDetails);
                    }
                    else
                    {
                        return NotFound(); // No user found with the specified email
                    }
                }
                catch (Exception ex)
                {
                    return NotFound("Could not find user");
                }
            }
        }

    }
}

