
using DatabaseApi.Models;

using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;





[ApiController]
[Route("api/[controller]")]
public class UserController  : ControllerBase{

    private readonly string _connectionString;

    public UserController(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserModel>), 200)]
    public List<UserModel> GetUsers()
        {
            List<UserModel> users = new List<UserModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT UserID, FirstName, LastName RoleID FROM Users";

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


}