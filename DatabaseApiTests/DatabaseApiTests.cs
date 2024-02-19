using System.Net;
using DatabaseApiCode.Controllers;
using DatabaseApiCode.Models;
using RestSharp;

namespace DatabaseApiCodeTests
{
    public class Tests
    {

        protected RestClient client;
        [SetUp]
        public void Setup()
        {
            client = new RestClient("http://localhost:5286/");
        }

        [Test]
        public void GetBbdSpendingsTest()
        {
            // Arrange
            var request = new RestRequest("api/BbdSpendings/{allocationYear}").AddUrlSegment("allocationYear", 2023);

            // Act
            var result = client.GetAsync<Dictionary<string, object>>((request)).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Values.Count, Is.EqualTo(3));
            Assert.True(result.ContainsKey("allocationYear"));
            Assert.True(result.ContainsKey("totalAmountAllocated"));
            Assert.True(result.ContainsKey("universityAllocations"));
        }

        
        [Test]
        public void GetAndPostBursaryAllocation()
        {
            // Arrange
            var requestGet = new RestRequest("api/BursaryAllocation");
            var requestPost = new RestRequest("api/BursaryAllocation");
            var requestPut = new RestRequest("api/UniversityApplication");

            UniversityApplicationModel universityApplicationModel =  new()
            {
                ApplicationID = 1,
                ApplicationStatusID = 2,
                AmountRequested = 200000,
                UniversityID = 1
            };
            requestPut.AddJsonBody(universityApplicationModel);

        
            BursaryAllocationModel bursaryAllocationModel = new()
            {
                UniversityID = 1,
                AllocatedYear = 2024,
                AmountAlloc = 200000,
            };
            requestPost.AddJsonBody(bursaryAllocationModel);

            // Act
            var resultPut = client.ExecutePutAsync((requestPut)).GetAwaiter().GetResult();
            var resultPost = client.ExecutePostAsync<BursaryAllocationModel>((requestPost)).GetAwaiter().GetResult();
            var resultGet = client.GetAsync<List<BursaryAllocationModel>>((requestGet)).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(resultPost);
            Assert.True(resultPut.StatusCode.Equals(HttpStatusCode.OK));
            Assert.NotNull(resultGet);
        }

        [Test]
        public void GetDepartmentsTest()
        {

            // Arrange
            var requestGet = new RestRequest("api/Departments");

            // Act
            var resultGet = client.GetAsync<List<string>>((requestGet)).GetAwaiter().GetResult();

            Assert.NotNull(resultGet);

        }

        [Test]
        public void GetStudentAllocationsTest()
        {

            // Arrange
            var requestGet = new RestRequest("api/StudentsAllocation");

            //Act
            var resultGet = client.GetAsync<List<StudentAllocationModel>>((requestGet)).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(resultGet);
        }

        [Test]
        public void GetStudentAllocationByIdTest()
        {

            // Arrange
            var requestGet = new RestRequest("api/StudentsAllocation/{allocationId}").AddUrlSegment("allocationId", 1);

            // Act
            var resultGet = client.GetAsync<StudentAllocationModel>((requestGet)).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(resultGet);
        }


        [Test]
        public void GetUniversitiesTest()
        {

            // Arrange
            var requestGet = new RestRequest("api/Universities");

            // Act
            var resultGet = client.GetAsync<List<string>>((requestGet)).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(resultGet);
        }

        [Test]
        public void GetUniversityApplicationTest()
        {

            // Arrange
            var requestGet = new RestRequest("api/UniversityApplication");

            // Act
            var resultGet = client.GetAsync<List<UniversityApplicationModel>>((requestGet)).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(resultGet);
        }


        [Test]
        public void GetUniversityApplicationByIdTest()
        {

            // Arrange
            var requestGet = new RestRequest("api/UniversityApplication/{applicationId}").AddUrlSegment("applicationId", 1);

            // Act
            var resultGet = client.GetAsync<UniversityApplicationModel>((requestGet)).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(resultGet);
        }

        [Test]
        public void GetUniversitySpendingsTest()
        {

            // Arrange
            var requestGet = new RestRequest("api/UniversitySpendings");

            // Act
            var resultGet = client.GetAsync<UniversitySpendingsModel>((requestGet)).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(resultGet);
            Assert.NotNull(resultGet.TotalAmount);
            Assert.NotNull(resultGet.AmountRemaining);
            Assert.NotNull(resultGet.AllocationYear);
        }

        [Test]
        public void GetUsersTest()
        {

            // Arrange
            var requestGet = new RestRequest("api/UniversityApplication");

            // Act
            var resultGet = client.GetAsync<object>((requestGet)).GetAwaiter().GetResult();

            // Assert
            Assert.NotNull(resultGet);
            Assert.Pass();
        }


        [TearDown]
        public void TearDown()
        {
            client = new RestClient();
        }
    }
}