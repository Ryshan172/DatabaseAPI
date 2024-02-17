using System;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DatabaseApi.Controllers
{    
    [ApiController]
    [Route("api/[controller]")]
    public class UserController  : ControllerBase
    {

        private readonly string _connectionString;

        public UserController(IConfiguration configuration)
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
                string query = "SELECT UserID, FirstName, LastName FROM Users";

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
    }
}