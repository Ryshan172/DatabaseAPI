using System;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserContactController : ControllerBase
    {

        private readonly string _connectionString;

        public UserContactController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet]
<<<<<<< HEAD:Controllers/UserContacts.Controller.cs
        [ProducesResponseType(typeof(IEnumerable<UserContactModel>), 200)]
         public List<UserContactModel> GetContacts()
=======
        [ProducesResponseType(typeof(IEnumerable<UserModel>), 200)]
        public List<UserContactModel> GetContacts()
>>>>>>> 4e6a5a1d5c66097ba112c370d09df1295f54ea80:Controllers/UserContactsController.cs
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
    }
}