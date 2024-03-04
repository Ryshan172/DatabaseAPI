
namespace DatabaseApiCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversitySpendingsController : ControllerBase

    {
        private readonly string _connectionString;

        public UniversitySpendingsController(IConfiguration configuration)

        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpGet]
        public IActionResult GetUniversitySpendingsAndStudents(int allocationYear, int universityID)
        {
            try {
                decimal totalAmount = 0;
                decimal amountRemaining = 0;
                List<StudentInfoModel> fundedStudents = new List<StudentInfoModel>();

                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Step 1: Get the total allocated amount for the university and year
                    string queryTotalAmount = @"
                    SELECT SUM(ba.AmountAlloc) AS TotalAmount
                    FROM BursaryAllocations ba
                    INNER JOIN UniversityApplication ua ON ba.UniversityApplicationID = ua.ApplicationID
                    WHERE ba.AllocationYear = @AllocationYear
                    AND ua.UniversityID = @UniversityID";

                    SqlCommand commandTotalAmount = new SqlCommand(queryTotalAmount, connection);
                    commandTotalAmount.Parameters.AddWithValue("@AllocationYear", allocationYear);
                    commandTotalAmount.Parameters.AddWithValue("@UniversityID", universityID);

                    object result = commandTotalAmount.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        totalAmount = (decimal)result;
                    }

                    // Step 2: Get the funded students and their allocation amounts
                    string queryFundedStudents = @"
                SELECT u.FirstName, u.LastName, st.StudentIDNum, sa.Amount AS AllocationAmount
                FROM StudentAllocations sa
                INNER JOIN StudentsTable st ON sa.StudentIDNum = st.StudentIDNum
                INNER JOIN Users u ON st.UserID = u.UserID
                WHERE sa.AllocationYear = @AllocationYear
                AND st.UniversityID = @UniversityID";


                    SqlCommand commandFundedStudents = new SqlCommand(queryFundedStudents, connection);
                    commandFundedStudents.Parameters.AddWithValue("@AllocationYear", allocationYear);
                    commandFundedStudents.Parameters.AddWithValue("@UniversityID", universityID);

                    using (SqlDataReader reader = commandFundedStudents.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string firstName = reader["FirstName"].ToString().Trim();
                            string lastName = reader["LastName"].ToString().Trim();
                            string studentIDNum = reader["StudentIDNum"].ToString();
                            decimal allocationAmount = Convert.ToDecimal(reader["AllocationAmount"]);

                            fundedStudents.Add(new StudentInfoModel
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                StudentIDNum = studentIDNum,
                                AllocationAmount = allocationAmount
                            });
                        }
                    }
                }

                // Calculate the remaining amount
                amountRemaining = totalAmount - fundedStudents.Sum(student => student.AllocationAmount);

                // Create and return the response model
                UniversitySpendingsModel response = new UniversitySpendingsModel
                {
                    AllocationYear = allocationYear,
                    UniversityID = universityID,
                    TotalAmount = totalAmount,
                    AmountRemaining = amountRemaining,
                    FundedStudents = fundedStudents
                };

                return Ok(response);
            } catch {
                return BadRequest("Could not find university spendings and students");
            }
            
        }


    
    }
}
