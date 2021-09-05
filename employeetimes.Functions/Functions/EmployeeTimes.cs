using employeetimes.common.Models;
using employeetimes.common.Responses;
using employeetimes.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace employeetimes.Functions.Functions
{
    public static class EmployeeTimes
    {
        [FunctionName(nameof(CreateTimeRecord))]
        public static async Task<IActionResult> CreateTimeRecord(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "time-record")] HttpRequest req,
             [Table("timerecord", Connection = "AzureWebJobsStorage")] CloudTable timerecordTable,
             ILogger log)
        {
            log.LogInformation("Recieved a new time record.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            TimeRecord timeRecord = JsonConvert.DeserializeObject<TimeRecord>(requestBody);

            if (timeRecord?.EmployeeId == null || timeRecord?.Date == null || timeRecord.Type == null)
            {

                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = " The request must have a employeeid, date and type."
                });
            }

            TimeRecordEntity timeRecordEntity = new TimeRecordEntity
            {
                EmployeeId = timeRecord.EmployeeId,
                Date = timeRecord.Date,
                Type = timeRecord.Type,
                ETag = "*",
                PartitionKey = "TIMERECORD",
                RowKey = Guid.NewGuid().ToString()

            };

            TableOperation addOperation = TableOperation.Insert(timeRecordEntity);
            await timerecordTable.ExecuteAsync(addOperation);

            string message = "New employee time record";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = timeRecordEntity
            });
        }
    }
}