#pragma warning disable CS8601
#pragma warning disable CS1591
#pragma warning disable CS8618

namespace DatabaseApiCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class UsersContactsController : ControllerBase

    {

        private readonly string _connectionString;


        public UsersContactsController(IConfiguration configuration)

        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserModel>), 200)]

        public List<UserContactModel> GetContacts()

        {
            List<UserContactModel> contactdetails = new List<UserContactModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT ContactID, UserID, Email, PhoneNumber FROM ContactDetails";

                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UserContactModel contacts = new UserContactModel
                        {
                            ContactID = reader.GetInt32(0),
                            UserID = reader.GetInt32(1),
                            Email = reader.GetString(2),
                            PhoneNumber = reader.GetString(3)
                        };

                        contactdetails.Add(contacts);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                }
            }

            return contactdetails;
        }


        [HttpPost]
        public async Task<IActionResult> AddContacts([FromBody] UserContactModel userContactModel)
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

                    var sql = "INSERT INTO ContactDetails (UserID, Email, PhoneNumber) VALUES (@UserID, @Email, @PhoneNumber)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userContactModel.UserID);
                        command.Parameters.AddWithValue("@Email", userContactModel.Email);
                        command.Parameters.AddWithValue("@PhoneNumber", userContactModel.PhoneNumber);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("User Contacts added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        
        // Search for contact details by UserID
        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(IEnumerable<UserContactModel>), 200)]
        [ProducesResponseType(404)]
        public IActionResult GetContactsByUserId(int userId)
        {
            List<UserContactModel> contactDetails = new List<UserContactModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT ContactID, UserID, Email, PhoneNumber FROM ContactDetails WHERE UserID = @UserID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        UserContactModel contact = new UserContactModel
                        {
                            ContactID = reader.GetInt32(0),
                            UserID = reader.GetInt32(1),
                            Email = reader.GetString(2),
                            PhoneNumber = reader.GetString(3)
                        };

                        contactDetails.Add(contact);
                    }

                    reader.Close();

                    if (contactDetails.Count > 0)
                    {
                        return Ok(contactDetails);
                    }
                    else
                    {
                        return NotFound(); // No contacts found for the specified user ID
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return StatusCode(500, "An error occurred while processing the request.");
                }
            }
        }

    }


    
}