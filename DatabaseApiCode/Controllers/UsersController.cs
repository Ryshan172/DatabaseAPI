#pragma warning disable CS8601
#pragma warning disable CS1591
#pragma warning disable CS8618
#pragma warning disable CS8600

namespace DatabaseApiCode.Controllers
{    
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController  : ControllerBase
    {
        private readonly string _connectionString;

        public UsersController(IConfiguration configuration)

        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] UserModel userModel)

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

                    var sql = "INSERT INTO Users (FirstName, LastName, RoleID) VALUES (@FirstName, @LastName, @RoleID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", userModel.FirstName);
                        command.Parameters.AddWithValue("@LastName", userModel.LastName);
                        command.Parameters.AddWithValue("@RoleID", userModel.RoleID);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("User added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserModel>), 200)]
        public  List<UserModel> GetUsers()

        {
            List<UserModel> users = new List<UserModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT UserID, FirstName, LastName, RoleID FROM Users";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UserModel user = new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            RoleID = reader.GetInt32(3)
                        };

                        users.Add(user);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                }
            }

            return users;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserModel), 200)]
        public IActionResult GetUserById(int id)
        {
            UserModel user = null;

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT UserID, FirstName, LastName, RoleID FROM Users WHERE UserID = @UserID";
                
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", id);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            UserID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            RoleID = reader.GetInt32(3)
                        };
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return StatusCode(500, "An error occurred while fetching the user.");
                }
            }

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

    }
}