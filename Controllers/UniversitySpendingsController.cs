

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;

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
                SUM(B.AmountAlloc) AS TotalAmountSpent
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

        decimal totalAmountSpent = 0;
        List<StudentAllocationInfo> fundedStudents = new List<StudentAllocationInfo>();

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AllocationYear", allocationYear);
                command.Parameters.AddWithValue("@UniversityID", universityID);

                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    // Process the first result set (university spendings)
                    if (reader.Read())
                    {
                        totalAmountSpent = Convert.ToDecimal(reader["TotalAmountSpent"]);
                    }

                    // Move to the next result set
                    reader.NextResult();

                    // Process the second result set (student allocation details)
                    while (reader.Read())
                    {
                        string firstName = reader["FirstName"].ToString();
                        string lastName = reader["LastName"].ToString();
                        decimal allocationAmount = Convert.ToDecimal(reader["AllocationAmount"]);
                        fundedStudents.Add(new StudentAllocationInfo
                        {
                            FirstName = firstName,
                            LastName = lastName,
                            AllocationAmount = allocationAmount
                        });
                    }
                }
            }
        }

        return Ok(new
        {
            AllocationYear = allocationYear,
            UniversityID = universityID,
            TotalAmountSpent = totalAmountSpent,
            FundedStudents = fundedStudents
        });
    }
}

public class StudentAllocationInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public decimal AllocationAmount { get; set; }
}
