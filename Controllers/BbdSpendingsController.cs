using System;
using System.Data;
using System.Data.SqlClient;
// using System.Web.Http;
using DatabaseApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
// using Microsoft.AspNetCore.Mvc; 


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
    public IActionResult GetAllocatedAmountTotal(int allocationYear)
    {
        string query = "SELECT SUM(AmountAlloc) AS TotalAmountAlloc FROM BursaryAllocations WHERE AllocationYear = @AllocationYear";

        decimal totalAllocatedAmount = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@AllocationYear", allocationYear);

                connection.Open();

                object result = command.ExecuteScalar();

                if (result != null && result != DBNull.Value)
                {
                    totalAllocatedAmount = Convert.ToDecimal(result);
                }
            }
        }

        return Ok(new { AllocationYear = allocationYear, TotalAllocatedAmount = totalAllocatedAmount });
    }
}