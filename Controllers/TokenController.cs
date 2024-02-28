namespace DatabaseApiCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly string _connectionString;

        public TokenController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost("generateToken")]
        public IActionResult GenerateToken(string studentIDNum)
        {
            if (studentIDNum.Length <= 0)
            {
                return BadRequest("Please add StudentIDNum.");
            }

            // Generate a unique token
            string token = Guid.NewGuid().ToString();

            // Calculate the expiry date (1 week from now)
            DateTime expiryDate = DateTime.Now.AddDays(7);

            //Expire 10 Seconds
            //DateTime expiryDate = DateTime.Now.AddSeconds(10);

            // Construct the URL with the token
            string url = $"https://ukukhulabursary.netlify.app/Student/Documents?token={token}";

            // Save the token and URL in the database
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "INSERT INTO TemporaryLinks (StudentIDNum, Token, TempLink, ExpiryDate) VALUES (@StudentIDNum, @Token, @TempLink, @ExpiryDate)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@StudentIDNum", studentIDNum);
                command.Parameters.AddWithValue("@Token", token);
                command.Parameters.AddWithValue("@TempLink", url);
                command.Parameters.AddWithValue("@ExpiryDate", expiryDate);

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        // Return the URL in the response
                        return Ok(new { TokenUrl = url });
                    }
                    else
                    {
                        return StatusCode(500, "Failed to generate token.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return StatusCode(500, "An error occurred while processing the request.");
                }
            }
        }


        [HttpGet("validateToken")]
        public IActionResult ValidateToken(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return BadRequest("Invalid URL.");
            }

            // Extract token from the URL
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return BadRequest("Invalid URL format.");
            }

            string token = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("token");
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token not found in the URL.");
            }

            // Retrieve the token details from the database
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT StudentIDNum, ExpiryDate FROM TemporaryLinks WHERE Token = @Token";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Token", token);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        string studentIDNum = reader.GetString(0);
                        DateTime expiryDate = reader.GetDateTime(1);

                        // Check if the token is expired
                        if (DateTime.Now <= expiryDate)
                        {
                            // Token is valid, return success
                            return Ok(new { StudentIDNum = studentIDNum, Token = token, IsValid = true });
                        }
                        else
                        {
                            // Token has expired
                            return Ok(new { IsValid = false, Message = "Token has expired." });
                        }
                    }
                    else
                    {
                        // Token not found in the database
                        return Ok(new { IsValid = false, Message = "Token not found." });
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