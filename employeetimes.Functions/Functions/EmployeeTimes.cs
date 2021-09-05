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


        [FunctionName(nameof(UpdateTimeRecord))]
        public static async Task<IActionResult> UpdateTimeRecord(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "time-record/{id}")] HttpRequest req,
            [Table("timerecord", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for time record: {id}, received");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TimeRecord timeRecord = JsonConvert.DeserializeObject<TimeRecord>(requestBody);

            TableOperation findOperation = TableOperation.Retrieve<TimeRecordEntity>("TIMERECORD", id);
            TableResult findResult = await todoTable.ExecuteAsync(findOperation);

            if (findResult.Result == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "TimeRecord not found."
                });
            }

            TimeRecordEntity timeRecordEntity = (TimeRecordEntity)findResult.Result;

            if (!(timeRecord?.Date == null))
            {
                timeRecordEntity.Date = timeRecord.Date;
            }

            if (!(timeRecord?.Type == null))
            {
                timeRecordEntity.Type = timeRecord.Type;
            }

            if (!(timeRecord?.EmployeeId == null))
            {
                timeRecordEntity.EmployeeId = timeRecord.EmployeeId;
            }
            TableOperation replaceOperation = TableOperation.Replace(timeRecordEntity);
            await todoTable.ExecuteAsync(replaceOperation);

            string message = $"TimeRecord: {id}, updated in table";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = timeRecordEntity
            });
        }


        [FunctionName(nameof(GetAllTimeRecords))]
        public static async Task<IActionResult> GetAllTimeRecords(
             [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "time-record")] HttpRequest req,
             [Table("timerecord", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
             ILogger log)
        {
            log.LogInformation("Get all time records received.");

            TableQuery<TimeRecordEntity> query = new TableQuery<TimeRecordEntity>();
            TableQuerySegment<TimeRecordEntity> timeRecords = await todoTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all todos.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = timeRecords
            });
        }

    }
}
