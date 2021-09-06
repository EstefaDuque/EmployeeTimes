using employeetimes.Functions.Entities;
using employeetimes.Functions.Functions;
using employeetimes.Test.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Xunit;

namespace employeetimes.Test.Tests
{

    public class ConsolidatedAPITest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();

        [Fact]
        public async void GetTimeRecordByDate_Should_Return_200()
        { //Arrange
            MockCloudTableConsolidated mockConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            List<ConsolidatedEntity> consolidatedRequest = TestFactory.GetConsolidatedListEntity();
            DateTime date = DateTime.UtcNow;
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(date);

            //Act
            IActionResult response = await ConsolidatedAPI.GetConsolidatedByDate(request, mockConsolidated, date, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
        }
    }
}
