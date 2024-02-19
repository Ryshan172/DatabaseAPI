using System.Net;
using System.Runtime.CompilerServices;
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
            Assert.IsInstanceOf<List<string>>(resultGet, "Result is not of type 'List'");

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
            Assert.IsInstanceOf<List<StudentAllocationModel>>(resultGet);
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
            Assert.IsInstanceOf<StudentAllocationModel>(resultGet);
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
            Assert.IsInstanceOf<List<string>>(resultGet);
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
            Assert.IsInstanceOf<List<UniversityApplicationModel>>(resultGet);
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
            Assert.IsInstanceOf<UniversityApplicationModel>(resultGet);
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
            Assert.IsInstanceOf<UniversitySpendingsModel>(resultGet);
        }

        [Test]
        public void GetAndPostUsersTest()
        {

            // Arrange
            var requestGet = new RestRequest("api/Users");
            var requestPost = new RestRequest("api/Users");
            var requestGetById = new RestRequest("api/Users/{id}").AddUrlSegment("id", 1);

            UserModel userModel = new()
            {
                FirstName = "ThisIs",
                LastName = "Test",
                RoleID = 2
            };
            requestPost.AddJsonBody(userModel);

            // Act
            var resultGet = client.GetAsync<List<UserModel>>((requestGet)).GetAwaiter().GetResult();
            var resultPost = client.ExecutePostAsync<UserModel>((requestPost)).GetAwaiter().GetResult();
            var resultGetAdded = client.GetAsync<List<UserModel>>((requestGet)).GetAwaiter().GetResult();
            

            // Assert
            Assert.That(resultPost.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.NotNull(resultGet);
            Assert.That(resultGetAdded, Has.Count.EqualTo(resultGet.Count + 1));
            foreach (UserModel user in resultGet)
            {
                Assert.NotNull(user.FirstName);
                Assert.NotNull(user.LastName);
                Assert.NotNull(user.UserID);
                Assert.NotNull(user.RoleID);
            }
        }

        [TearDown]
        public void TearDown()
        {
            client = new RestClient();
        }
    }
}