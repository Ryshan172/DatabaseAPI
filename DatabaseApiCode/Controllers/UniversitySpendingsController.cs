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
            string query = @"
                SELECT 
                    U.UniName,
                    SUM(B.AmountAlloc) AS TotalAmount
                FROM 
                    Universities U
                INNER JOIN 
                    BursaryAllocations B ON U.UniversityID = B.UniversityID
                WHERE 
                    B.AllocationYear = @AllocationYear AND
                    U.UniversityID = @UniversityID
                GROUP BY 
                    U.UniName;

                SELECT 
                    U.UserID,
                    U.FirstName,
                    U.LastName,
                    SA.Amount AS AllocationAmount
                FROM 
                    Users U
                INNER JOIN 
                    StudentsTable S ON U.UserID = S.UserID
                INNER JOIN 
                    StudentAllocations SA ON S.StudentID = SA.StudentID
                INNER JOIN 
                    BursaryAllocations B ON SA.AllocationID = B.AllocationID
                WHERE 
                    B.AllocationYear = @AllocationYear AND
                    B.UniversityID = @UniversityID;";

            decimal totalAmount = 0;
            decimal AmountRemaining =0;
            List<StudentInfoModel> fundedStudents = new List<StudentInfoModel>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@AllocationYear", allocationYear);
                    command.Parameters.AddWithValue("@UniversityID", universityID);

                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        
                        if (reader.Read())
                        {
                            totalAmount = Convert.ToDecimal(reader["TotalAmount"]);
                        }

                        
                        reader.NextResult();

                    
                        while (reader.Read())
                        {
                            string firstName = reader["FirstName"].ToString();
                            string lastName = reader["LastName"].ToString();
                            decimal allocationAmount = Convert.ToDecimal(reader["AllocationAmount"]);
                            AmountRemaining = totalAmount - allocationAmount;

                            fundedStudents.Add(new StudentInfoModel
                            {
                                FirstName = firstName,
                                LastName = lastName,
                                AllocationAmount = allocationAmount
                                // AmountRemaining = AmountRemaining

                            });
                        }
                    }
                }
            }

            return Ok(new UniversitySpendingsModel()
            {
                AllocationYear = allocationYear,
                UniversityID = universityID,
                TotalAmount = totalAmount,
                FundedStudents = fundedStudents,
                AmountRemaining =AmountRemaining ,

            });
        }
    }
}
