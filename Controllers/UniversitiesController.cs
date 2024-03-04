
namespace DatabaseApiCode.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UniversitiesController : ControllerBase
    {
        private readonly string _connectionString;

        public UniversitiesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost]
        public async Task<IActionResult> AddUniversity([FromBody] UniversitiesModel universitiesModel)
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

                    // Check if the university name already exists
                    var checkSql = "SELECT COUNT(*) FROM Universities WHERE UniName = @UniName";
                    using (var checkCommand = new SqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@UniName", universitiesModel.UniName);
                        var existingCount = (int)await checkCommand.ExecuteScalarAsync();

                        // If a university with the same name already exists, return a conflict response
                        if (existingCount > 0)
                        {
                            return Conflict("University with the same name already exists");
                        }
                    }

                    // If the name is not already in the database, insert the new university
                    var sql = "INSERT INTO Universities (UniName) VALUES (@UniName)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UniName", universitiesModel.UniName);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("University added successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("University could not be added");
            }
        }



        [HttpGet]
        public async Task<IActionResult> GetAllUniversities()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT UniversityID, UniName FROM Universities";
                    using (var command = new SqlCommand(sql, connection))
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        var universitiesList = new List<UniversitiesModel>();
                        while (await reader.ReadAsync())
                        {
                            var universitiesGet = new UniversitiesModel
                            {   
                                // Needed to change the model to decimal
                                UniversityID = reader.GetInt32(0),
                                UniName = reader.GetString(1).Trim(),
                                
                            };
                            universitiesList.Add(universitiesGet);
                        }
                        return Ok(universitiesList);
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("could not find universities");
            }
        }

        // Get Universities ByID 
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUniversityByID(int id)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT UniversityID, UniName FROM Universities WHERE UniversityID = @UniversityID";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@UniversityID", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var university = new UniversitiesModel
                                {
                                    UniversityID = reader.GetInt32(0),
                                    UniName = reader.GetString(1)
                                };
                                return Ok(university);
                            }
                            else
                            {
                                return NotFound("University not found");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return NotFound("University not found");
            }
        }



        
    }
}
