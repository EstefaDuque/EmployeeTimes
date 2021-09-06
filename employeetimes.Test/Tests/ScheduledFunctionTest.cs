using employeetimes.Functions.Functions;
using employeetimes.Test.Helpers;
using System;
using Xunit;


namespace employeetimes.Test.Tests
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public void ScheduledFunction_Should_Log_Message()
        {
            //Arrange
            MockCloudTableTimeRecord mockTimeRecord = new MockCloudTableTimeRecord(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);
            MockCloudTableConsolidated mockConsolidated = new MockCloudTableConsolidated(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            //Act
            ScheduleFunction.RunAsync(null, mockTimeRecord, mockConsolidated, logger);
            string message = logger.Logs[0];

            //Assert
            Assert.Contains("Consolidated function", message);

        }
    }
}
