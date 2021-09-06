using employeetimes.common.Responses;
using employeetimes.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace employeetimes.Functions.Functions
{
    public static class ConsolidatedAPI
    {
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
