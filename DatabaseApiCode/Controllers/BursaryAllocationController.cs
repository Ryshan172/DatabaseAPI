namespace DatabaseApiCode.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class BursaryAllocationController : ControllerBase
    {
        private readonly string _connectionString;


        public BursaryAllocationController(IConfiguration configuration)

        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BursaryAllocationModel>), 200)]
        public List<BursaryAllocationModel> GetBursaryAllocation(int allocationYear)
        {
            List<BursaryAllocationModel> allocations = new List<BursaryAllocationModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                string query = "SELECT UniversityID, AmountAlloc, AllocationYear FROM BursaryAllocations WHERE AllocationYear = @AllocationYear";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@AllocationYear", allocationYear);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        BursaryAllocationModel allocation = new BursaryAllocationModel
                        {
                            UniversityID = reader.GetInt32(0),
                            AmountAlloc = reader.GetDecimal(1),
                            AllocatedYear = reader.GetInt32(2)
                        };

                        allocations.Add(allocation);
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    
                }
            }

            return allocations;
        }


        [HttpPost]
        public async Task<IActionResult> AddBursaryAllocation([FromBody] BursaryAllocationModel bursaryallocation)
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

                    var sql = @"
                        INSERT INTO BursaryAllocations (AmountAlloc, AllocationYear, UniversityID)
                        VALUES (@UniversityID ,@AmountAlloc, @AllocationYear )";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@AmountAlloc", bursaryallocation.AmountAlloc);
                        command.Parameters.AddWithValue("@AllocationYear", bursaryallocation.AllocatedYear);
                        command.Parameters.AddWithValue("@UniversityID", bursaryallocation. UniversityID);

                        
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Bursary amount Allocation added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
















