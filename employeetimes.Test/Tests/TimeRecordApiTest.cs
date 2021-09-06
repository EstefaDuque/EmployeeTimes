using employeetimes.common.Models;
using employeetimes.Functions.Entities;
using employeetimes.Functions.Functions;
using employeetimes.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Xunit;
namespace employeetimes.Test.Tests
{
    public class TimeRecordApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void CreateTimeRecord_Should_Return_200()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecords = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimeRecord timeRecordRequest = TestFactory.GetTimeRecordRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRecordRequest);

            //Act
            IActionResult response = await EmployeeTimesAPI.CreateTimeRecord(request, mockTimeRecords, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void UpdateTimeRecord_Should_Return_200()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecords = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimeRecord timeRecordRequest = TestFactory.GetTimeRecordRequest();
            Guid timeRecordKey = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRecordKey, timeRecordRequest);

            //Act
            IActionResult response = await EmployeeTimesAPI.UpdateTimeRecord(request, mockTimeRecords, timeRecordKey.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        [Fact]
        public async void DeleteTimeRecord_Should_Return_200()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecords = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimeRecordEntity timeRecordRequest = TestFactory.GetTimeRecordEntity();
            Guid timeRecordKey = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRecordKey);

            //Act
            IActionResult response = await EmployeeTimesAPI.DeleteTimeRecord(request, timeRecordRequest, mockTimeRecords, timeRecordKey.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        [Fact]
        public async void GetTimeRecord_Should_Return_200()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecords = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimeRecordEntity timeRecordRequest = TestFactory.GetTimeRecordEntity();
            Guid timeRecordKey = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRecordKey);

            //Act
            IActionResult response =  EmployeeTimesAPI.GetTimeRecordById(request, timeRecordRequest, timeRecordKey.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        /*
        [Fact]
        public async void GetTimeRecordByDate_Should_Return_200()
        {
            //Arrange
            MockCloudTableConsolidated mockConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ConsolidatedEntity consolidatedRequest = TestFactory.GetConsolidatedEntity();
            DateTime date = DateTime.UtcNow;
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(date);

            //Act
            IActionResult response = await EmployeeTimesAPI.GetConsolidatedByDate(request, mockConsolidated, date, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }

        */
        
        /*
        [Fact]
        public async void GetAllTimeRecords_Should_Return_200()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecords = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            TimeRecord timeRecordRequest = TestFactory.GetTimeRecordRequest();
            Guid timeRecordKey = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(timeRecordRequest);

            //Act
            IActionResult response = await EmployeeTimesAPI.GetAllTimeRecords(request, mockTimeRecords, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
        */
    }
}
