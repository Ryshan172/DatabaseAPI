
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
                return BadRequest("Could not add user");
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
                    return NotFound("Could not find user");

                }
            }

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }


        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUserByID(int userId, [FromBody] UserModel userModel)
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

                    var sql = "UPDATE Users SET FirstName = @FirstName, LastName = @LastName, RoleID = @RoleID WHERE UserID = @UserID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@FirstName", userModel.FirstName);
                        command.Parameters.AddWithValue("@LastName", userModel.LastName);
                        command.Parameters.AddWithValue("@RoleID", userModel.RoleID);
                        command.Parameters.AddWithValue("@UserID", userId);
                        int rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return NotFound($"User with ID {userId} not found.");
                        }
                    }
                }

                return Ok($"User with ID {userId} updated successfully");
            }
            catch (Exception ex)
            {
                return NotFound("User not found.");
            }
        }


        [HttpGet("users/{roleid}")]
        [ProducesResponseType(typeof(List<UserWithContactModel>), 200)]
        public IActionResult GetUsersByRoleID(int roleid)
        {
            List<UserWithContactModel> users = new List<UserWithContactModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string roleIdQuery = "SELECT RoleID FROM Roles WHERE RoleID = @RoleID";
                SqlCommand roleIdCommand = new SqlCommand(roleIdQuery, connection);
                roleIdCommand.Parameters.AddWithValue("@RoleID", roleid);

                string usersQuery = @"
                    SELECT u.UserID, u.FirstName, u.LastName, u.RoleID, c.Email, c.PhoneNumber
                    FROM Users u
                    INNER JOIN ContactDetails c ON u.UserID = c.UserID
                    WHERE u.RoleID = @RoleID";

                try
                {
                    connection.Open();

                    // Get RoleID by RoleName
                    int roleId = Convert.ToInt32(roleIdCommand.ExecuteScalar());
                    if (roleId == 0)
                    {
                        return NotFound($"Role '{roleid}' not found.");
                    }

                    // Retrieve users with the given RoleID
                    SqlCommand command = new SqlCommand(usersQuery, connection);
                    command.Parameters.AddWithValue("@RoleID", roleId);

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UserWithContactModel user = new UserWithContactModel
                        {
                            UserID = reader.GetInt32(0),
                            FirstName = reader.GetString(1).Trim(),
                            LastName = reader.GetString(2).Trim(),
                            RoleID = reader.GetInt32(3),
                            Email = reader.IsDBNull(4) ? null : reader.GetString(4).Trim(),
                            PhoneNumber = reader.IsDBNull(5) ? null : reader.GetString(5)
                        };
                        users.Add(user);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    return NotFound("User not found.");
                }
            }

            return Ok(users);
        }


        [HttpPost("createBBDUser")]
        public IActionResult CreateBBDUser([FromBody] CreateUserModel model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("EXEC [dbo].[CreateBBDUser] @PhoneNumber, @Email, @FirstName, @LastName", connection))
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
                        command.Parameters.AddWithValue("@LastName", model.LastName);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("User created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Could not create user");
            }
        }


        [HttpPost("createUniversityUser")]
        public IActionResult CreateUniversityUser([FromBody] CreateUniversityUserModel model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand("EXEC [dbo].[CreateUniversityUser] @PhoneNumber, @Email, @FirstName, @LastName, @DepartmentID, @UniversityID", connection))
                    {
                        command.Parameters.AddWithValue("@PhoneNumber", model.PhoneNumber);
                        command.Parameters.AddWithValue("@Email", model.Email);
                        command.Parameters.AddWithValue("@FirstName", model.FirstName);
                        command.Parameters.AddWithValue("@LastName", model.LastName);
                        command.Parameters.AddWithValue("@DepartmentID", model.DepartmentID);
                        command.Parameters.AddWithValue("@UniversityID", model.UniversityID);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok("University user created successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Could not create University user");
            }
        }


        [HttpGet("universityUserDetails/{userId}")]
        public async Task<IActionResult> GetUniversityUserDetails(int userId)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"
                        SELECT DepartmentID, UniversityID
                        FROM UniversityUser
                        WHERE UserID = @UserId";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", userId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                var departmentId = reader.GetInt32(reader.GetOrdinal("DepartmentID"));
                                var universityId = reader.GetInt32(reader.GetOrdinal("UniversityID"));

                                var userDetails = new
                                {
                                    DepartmentID = departmentId,
                                    UniversityID = universityId,
                                    UserID = userId
                                };

                                return Ok(userDetails);
                            }
                            else
                            {
                                return NotFound("User not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("User not found");
            }
        }


    }




    
}