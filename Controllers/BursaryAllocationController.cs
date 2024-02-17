
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using DatabaseApi.Models;

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
                        AmountAllocated = reader.GetDecimal(1),
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
}

















