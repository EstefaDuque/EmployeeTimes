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
                Date = ((DateTime)timeRecord.Date).AddHours(-5),
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
            [Table("timerecord", Connection = "AzureWebJobsStorage")] CloudTable timeRecordTable,
            string id,
            ILogger log)
        {
            log.LogInformation($"Update for time record: {id}, received");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TimeRecord timeRecord = JsonConvert.DeserializeObject<TimeRecord>(requestBody);

            TableOperation findOperation = TableOperation.Retrieve<TimeRecordEntity>("TIMERECORD", id);
            TableResult findResult = await timeRecordTable.ExecuteAsync(findOperation);

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

                timeRecordEntity.Date = ((DateTime)timeRecord.Date).AddHours(-5);

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
            await timeRecordTable.ExecuteAsync(replaceOperation);

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
             [Table("timerecord", Connection = "AzureWebJobsStorage")] CloudTable timeRecordTable,
             ILogger log)
        {
            log.LogInformation("Get all time records received.");

            TableQuery<TimeRecordEntity> query = new TableQuery<TimeRecordEntity>();
            TableQuerySegment<TimeRecordEntity> timeRecords = await timeRecordTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all time records.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = timeRecords
            });
        }

        [FunctionName(nameof(GetTimeRecordById))]
        public static IActionResult GetTimeRecordById(
             [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "time-record/{id}")] HttpRequest req,
             [Table("timerecord", "TIMERECORD", "{id}", Connection = "AzureWebJobsStorage")] TimeRecordEntity timeRecordEntity,
             string id,
             ILogger log)
        {
            log.LogInformation($"Get time record by id : {id}");

            if (timeRecordEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Time record not found."
                });
            }

            string message = $"Time record: {timeRecordEntity.RowKey} retrieved";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = timeRecordEntity
            });


        }


        [FunctionName(nameof(DeleteTimeRecord))]
        public static async Task<IActionResult> DeleteTimeRecord(
             [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "time-record/{id}")] HttpRequest req,
             [Table("timerecord", "TIMERECORD", "{id}", Connection = "AzureWebJobsStorage")] TimeRecordEntity timeRecordEntity,
             [Table("timerecord", Connection = "AzureWebJobsStorage")] CloudTable TimeRecordTable,

             string id,
              ILogger log)
        {
            log.LogInformation($"Delete time record id : {id}, received");

            if (timeRecordEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Time record not found."
                });
            }

            await TimeRecordTable.ExecuteAsync(TableOperation.Delete(timeRecordEntity));

            string message = $"Time record: {timeRecordEntity.RowKey} deleted";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = timeRecordEntity
            });
        }




        [FunctionName(nameof(GetConsolidatedByDate))]
        public static async Task<IActionResult> GetConsolidatedByDate(
             [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "consolidated/{date}")] HttpRequest req,
             [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
             DateTime date,
             ILogger log)
        {
            string filter = TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.Equal, date);

            TableQuery<ConsolidatedEntity> query = new TableQuery<ConsolidatedEntity>().Where(filter);
            TableQuerySegment<ConsolidatedEntity> consolidatedTimeRecords = await consolidatedTable.ExecuteQuerySegmentedAsync(query, null);

            string message = $"Get all consolidated by date: {date}";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = consolidatedTimeRecords
            });
        }
    }
}
