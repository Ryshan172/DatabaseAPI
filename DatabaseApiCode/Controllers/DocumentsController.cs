
namespace DatabaseApiCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class DocumentsController : ControllerBase

    {

        private readonly string _connectionString;


        public DocumentsController(IConfiguration configuration)

        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }


        [HttpPost]
        public async Task<IActionResult> AddStudentDocuments([FromBody] DocumentsModel documentsModel)
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

                    var sql = "INSERT INTO Documents (StudentIDNum, AcademicTranscript, ID) VALUES (@StudentIDNum, @AcademicTranscript, @ID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StudentIDNum", documentsModel.StudentIDNum);
                        command.Parameters.AddWithValue("@AcademicTranscript", documentsModel.AcademicTranscript);
                        command.Parameters.AddWithValue("@ID", documentsModel.ID);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Student Documents added successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
        
        
        

    }


    
}