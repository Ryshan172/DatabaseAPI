
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
                return StatusCode(500, $"An error occurred: {ex.Message}");
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
                                UniName = reader.GetString(1),
                                
                            };
                            universitiesList.Add(universitiesGet);
                        }
                        return Ok(universitiesList);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
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
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        
    }
}
