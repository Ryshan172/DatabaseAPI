#pragma warning disable CS8618 
#pragma warning disable CS1591
#pragma warning disable CS8601
#pragma warning disable CS8600
#pragma warning disable CS8604

// using DatabaseApiCode.Attributes;
// 
namespace DatabaseApiCode.Controllers
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


        [HttpGet("{allocationYear}")]
        public IActionResult GetSpendingForYear(int allocationYear)
        {
            string query = @"
                SELECT 
                    SUM(B.AmountAlloc) AS TotalAmountAlloc,
                    U.UniName,
                    BB.Budget AS TotalBudget
                FROM 
                    BursaryAllocations B
                INNER JOIN 
                    Universities U ON B.UniversityID = U.UniversityID
                LEFT JOIN 
                    BBDAdminBalance BB ON B.AllocationYear = BB.BudgetYear
                WHERE 
                    B.AllocationYear = @AllocationYear
                GROUP BY 
                    U.UniName, BB.Budget";

            decimal totalAmountAllocated = 0;
            decimal totalBudget = 0;
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

                            // Retrieve budget if available
                            if (reader["TotalBudget"] != DBNull.Value)
                            {
                                totalBudget = Convert.ToDecimal(reader["TotalBudget"]);
                            }

                            universityAllocations.Add(universityName, amountAllocated);
                            totalAmountAllocated += amountAllocated;
                        }
                    }
                }
            }

            return Ok(new
            {
                AllocationYear = allocationYear,
                TotalAmountAllocated = totalAmountAllocated,
                TotalBudget = totalBudget,
                AmountRemaining = totalBudget - totalAmountAllocated,
                UniversityAllocations = universityAllocations
            });
        }


        // Add Money to the BBD Budget Table 
        [HttpPost]
        public async Task<IActionResult> AddBudgetAmount([FromBody] BBDBudgetModel bBDBudgetModell)
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
                        INSERT INTO BBDAdminBalance (Budget, AmountAllocated, BudgetYear)
                        VALUES (@Budget, @AmountAllocated, @BudgetYear)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Budget", bBDBudgetModell.Budget);
                        command.Parameters.AddWithValue("@AmountAllocated", bBDBudgetModell.AmountAllocated);
                        command.Parameters.AddWithValue("@BudgetYear", bBDBudgetModell.BudgetYear);
                        
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Budget for year added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }

            
        }


        // Method to update the budget for a year
        [HttpPut("{budgetYear}")]
        public async Task<IActionResult> UpdateBudgetAmount(int budgetYear, [FromBody] BBDBudgetModel bBDBudgetModell)
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
                        UPDATE BBDAdminBalance
                        SET Budget = @Budget,
                            AmountAllocated = @AmountAllocated
                        WHERE BudgetYear = @BudgetYear";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Budget", bBDBudgetModell.Budget);
                        command.Parameters.AddWithValue("@AmountAllocated", bBDBudgetModell.AmountAllocated);
                        command.Parameters.AddWithValue("@BudgetYear", budgetYear); // Use the route parameter

                        var rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return NotFound($"Budget for year {budgetYear} does not exist");
                        }
                    }
                }

                return Ok($"Budget for year {budgetYear} updated successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


    }
}