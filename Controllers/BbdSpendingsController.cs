namespace DatabaseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class BbdSpendingsController : ControllerBase
    {
        private readonly string _connectionString;
        public BbdSpendingsController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpGet]
        public IActionResult GetAllocatedAmountAndUniversities(int allocationYear)

        {
            string query = @"
                SELECT 
                    SUM(B.AmountAlloc) AS TotalAmountAlloc,
                    U.UniName
                FROM 
                    BursaryAllocations B
                INNER JOIN 
                    Universities U ON B.UniversityID = U.UniversityID
                WHERE 
                    B.AllocationYear = @AllocationYear
                GROUP BY 
                    U.UniName";

            decimal totalAmountAllocated = 0;
            Dictionary<string, decimal> universityAllocations = new Dictionary<string, decimal>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AllocationYear", allocationYear);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string universityName = reader["UniName"].ToString();
                            decimal amountAllocated = Convert.ToDecimal(reader["TotalAmountAlloc"]);

                            universityAllocations.Add(universityName, amountAllocated);
                            totalAmountAllocated += amountAllocated;
                        }
                    }
                }
            }

            return Ok(new { AllocationYear = allocationYear, TotalAmountAllocated = totalAmountAllocated, UniversityAllocations = universityAllocations });
        }
    }
}