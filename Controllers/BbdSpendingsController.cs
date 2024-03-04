
// using DatabaseApiCode.Attributes;
// 
using System.Data;

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
            try {
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
            } catch(Exception e) {
                return NotFound("Could not retrieve spending for "+allocationYear);
            }
            
        }


        [HttpGet("GetCurrentBudget")]
        public IActionResult GetCurrentBudget()
        {
            try
            {
                DataTable GetDataTable(string query, SqlConnection connection)
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    return dataTable;
                }

                string query = @"SELECT * FROM BBDAdminBalance
                            WHERE BudgetYear = YEAR(GETDATE())";


                SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                DataRow Row = GetDataTable(query, connection).Rows[0];
                decimal Allocated = decimal.Parse(Row["AmountAllocated"].ToString());
                decimal Budget = decimal.Parse(Row["Budget"].ToString());
                decimal Remaining = decimal.Parse(Row["AmountRemaining"].ToString());
                int year = int.Parse(Row["BudgetYear"].ToString());

                return Ok(new
                {
                    Allocated = Allocated,
                    Budget = Budget,
                    Remaining = Remaining,
                    year = year,

                });
            } catch (Exception e) {
                return NotFound("Could not retrieve current budget");
            }
           
        }


        [HttpGet("GetTotalSpentOnAllUniversities")]
        public IActionResult GetTotalSpentOnAllUniversities()
        {
            try {
                DataTable GetDataTable(string query, SqlConnection connection)
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    return dataTable;
                }

                string query = @"SELECT 
                            SUM(B.AmountAlloc) AS TotalAmountAlloc,
                            U.UniName
                            FROM 
                            BursaryAllocations B
                            INNER JOIN 
                            Universities U ON B.UniversityID = U.UniversityID
                            LEFT JOIN 
                            BBDAdminBalance BB ON B.AllocationYear = BB.BudgetYear
                            GROUP BY 
                            U.UniName";


                SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();

                List<AllocationBudgetModel> bursaries = new List<AllocationBudgetModel>();
                DataTable Rows = GetDataTable(query, connection);
                foreach (DataRow Row in Rows.Rows)
                {
                    bursaries.Add(new AllocationBudgetModel
                    {
                        UniName = Row["UniName"].ToString(),
                        AmountAllocated = decimal.Parse(Row["TotalAmountAlloc"].ToString())

                    });
                }
                return Ok(bursaries);
            }
            catch{
                return NotFound("Could not retrieve total spent on all universities");
            }
           

        }




        [HttpGet("GetUniversityPaymentHistory")]
        public IActionResult GetUniversityPaymentHistory()
        {
            try {
                DataTable GetDataTable(string query, SqlConnection connection)
                {
                    DataTable dataTable = new DataTable();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                    return dataTable;
                }

                string query = @"SELECT * FROM BursaryAllocations
                            JOIN Universities
                            ON BursaryAllocations.UniversityID = Universities.UniversityID";


                SqlConnection connection = new SqlConnection(_connectionString);
                connection.Open();
                List<BursaryHistoryModel> bursaries = new List<BursaryHistoryModel>();
                DataTable Rows = GetDataTable(query, connection);
                foreach (DataRow Row in Rows.Rows)
                {
                    bursaries.Add(new BursaryHistoryModel
                    {
                        AmountAllocated = decimal.Parse(Row["AmountAlloc"].ToString()),
                        AllocationYear = int.Parse(Row["AllocationYear"].ToString()),
                        UniName = Row["UniName"].ToString(),
                        AllocationID = int.Parse(Row["AllocationID"].ToString())


                    });
                }


                return Ok(bursaries);
            } catch {
                return NotFound("Could not retrieve university payment history");
            }
            
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
                return BadRequest("Could not add to budget for the year");
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
                return BadRequest("Could update budget");
            }
        }



        [HttpPost("allocateFundsToUniversities")]
        public async Task<IActionResult> AllocateFundsToUniversities()
        {

            
         DataTable GetDataTable(string query, SqlConnection connection)
        {
            DataTable dataTable = new DataTable();
            SqlCommand command = new SqlCommand(query, connection);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            adapter.Fill(dataTable);
            return dataTable;
        }
            try
            {
                SqlConnection connection = new SqlConnection(_connectionString);

                await connection.OpenAsync();

                string BBDFund = @"SELECT AmountRemaining FROM BBDAdminBalance
                                   WHERE BudgetYear = YEAR(GETDATE())";

                string universityApplicationQuery = @"SELECT * FROM UniversityApplication
                               WHERE ApplicationYear = YEAR(GETDATE())
                               AND
                               ApplicationStatusID = 2";

                DataRow fundRow = GetDataTable(BBDFund, connection).Rows[0];
                decimal TotalBudget = decimal.Parse(fundRow["AmountRemaining"].ToString());


                DataTable universityApplicationData = GetDataTable(universityApplicationQuery, connection);


                decimal universityBudget = TotalBudget / universityApplicationData.Rows.Count;


                foreach (DataRow row in universityApplicationData.Rows)
                {

                    string insert = @"INSERT INTO [dbo].BursaryAllocations (UniversityID, 
                   AmountAlloc, AllocationYear, UniversityApplicationID)
                   VALUES (@UniversityID, @AmountAlloc, @AllocationYear, @UniversityApplicationID)";

                    SqlCommand command = new SqlCommand(insert, connection);
                    command.Parameters.AddWithValue("@UniversityID", int.Parse(row["UniversityID"].ToString()));
                    command.Parameters.AddWithValue("@AmountAlloc", universityBudget);
                    command.Parameters.AddWithValue("@AllocationYear", DateTime.Now.Year);
                    command.Parameters.AddWithValue("@UniversityApplicationID", int.Parse(row["ApplicationID"].ToString()));

                    command.ExecuteNonQuery();
                }


                return Ok("Funds have been allocated to all");
            }
            catch (Exception ex)
            {
                return BadRequest("Could not allocate funds");
            }
        }




    }
}