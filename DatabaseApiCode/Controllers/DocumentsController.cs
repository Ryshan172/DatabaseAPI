using Microsoft.Azure.Storage.Blob;
namespace DatabaseApiCode.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
 
    public class DocumentsController : ControllerBase
 
    {
 
        private readonly IConfiguration _configuration;  
        private readonly string _connectionString;
 
        public DocumentsController(IConfiguration configuration)
 
        {
            _configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
 
 
        [HttpPost(nameof(UploadStudentDocuments))]
        public async Task<IActionResult> UploadStudentDocuments(IFormFile academicTranscriptFile, IFormFile idFile, string studentIDNum)
        {
            try
            {
                if (string.IsNullOrEmpty(studentIDNum))
                {
                    return BadRequest("Invalid StudentIDNum");
                }

                string blobStorageConnection = _configuration.GetValue<string>("BlobStorageConnection");
                Microsoft.Azure.Storage.CloudStorageAccount cloudStorageAccount = Microsoft.Azure.Storage.CloudStorageAccount.Parse(blobStorageConnection);
                CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference(_configuration.GetValue<string>("BlobContainerName"));

                // Upload academic transcript file
                string academicTranscriptFileName = academicTranscriptFile.FileName;
                CloudBlockBlob academicTranscriptBlob = container.GetBlockBlobReference($"{Guid.NewGuid()}-{academicTranscriptFileName}");
                await using (var academicTranscriptData = academicTranscriptFile.OpenReadStream())
                {
                    await academicTranscriptBlob.UploadFromStreamAsync(academicTranscriptData);
                }
                string academicTranscriptFileUrl = academicTranscriptBlob.Uri.ToString();

                // Upload ID file
                string idFileName = idFile.FileName;
                CloudBlockBlob idBlob = container.GetBlockBlobReference($"{Guid.NewGuid()}-{idFileName}");
                await using (var idData = idFile.OpenReadStream())
                {
                    await idBlob.UploadFromStreamAsync(idData);
                }
                string idFileUrl = idBlob.Uri.ToString();

                // Store file details in the database
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "INSERT INTO Documents (StudentIDNum, AcademicTranscript, ID) VALUES (@StudentIDNum, @AcademicTranscript, @ID)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StudentIDNum", studentIDNum);
                        command.Parameters.AddWithValue("@AcademicTranscript", academicTranscriptFileUrl);
                        command.Parameters.AddWithValue("@ID", idFileUrl);
                        await command.ExecuteNonQueryAsync();
                    }
                }

                // Return the URLs in the response
                return Ok(new { Message = "Files Uploaded Successfully", AcademicTranscriptUrl = academicTranscriptFileUrl, IDUrl = idFileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        // Search for Student Allocation by ID
        [HttpGet("{studentIdNum}")]
        public async Task<IActionResult> GetStudentDocumentsByID(string studentIdNum)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    var sql = "SELECT StudentIDNum, AcademicTranscript, ID FROM Documents WHERE StudentIDNum = @StudentIDNum";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@StudentIDNum", studentIdNum);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var studentDocuments = new DocumentsModel
                                {
                                    StudentIDNum = reader.GetString(0),
                                    ID = reader.GetString(1),
                                    AcademicTranscript = reader.GetString(2),
                                    
                                };
                                return Ok(studentDocuments);
                            }
                            else
                            {
                                return NotFound(); // Documents for the specified StudentIDNum not found
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


 
        
        
 
    }
 
 
    
}