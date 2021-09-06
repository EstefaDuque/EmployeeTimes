using employeetimes.Functions.Entities;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace employeetimes.Functions.Functions
{
    public static class ScheduleFunction
    {
        [FunctionName("ScheduleFunction")]
        public static async Task RunAsync([TimerTrigger("* 0/1 * * * *")] TimerInfo myTimer,
            [Table("timerecord", Connection = "AzureWebJobsStorage")] CloudTable timeRecordTable,
            [Table("consolidated", Connection = "AzureWebJobsStorage")] CloudTable consolidatedTable,
            ILogger log)
        {
            log.LogInformation($"Consolidated function completed at: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("Consolidated", QueryComparisons.Equal, false);
            TableQuery<TimeRecordEntity> query = new TableQuery<TimeRecordEntity>().Where(filter);
            TableQuerySegment<TimeRecordEntity> timeRecordsFalse = await timeRecordTable.ExecuteQuerySegmentedAsync(query, null);

            IEnumerable<IGrouping<int?, TimeRecordEntity>> timeRecordsByEmployee =
                timeRecordsFalse.GroupBy(employee => employee.EmployeeId);

            foreach (IGrouping<int?, TimeRecordEntity> group in timeRecordsByEmployee)
            {
                List<TimeRecordEntity> groupByEmployee = group.OrderBy(user => user.Date).ToList();
                int lp = 0;
                foreach (TimeRecordEntity employee in groupByEmployee)
                {
                    if ((lp + 1) < groupByEmployee.Count)
                    {

                        TimeRecordEntity nextEmployee = groupByEmployee[lp + 1];
                        int subtractMinutes = nextEmployee.Date.Minute - employee.Date.Minute;
                        int subtractHours = (groupByEmployee[lp + 1].Date.Hour - employee.Date.Hour) * 60;
                        int minutesWork = subtractMinutes + subtractHours;
                        ConsolidatedEntity consolidatedEntity = new ConsolidatedEntity
                        {
                            EmployeeId = (int)employee.EmployeeId,
                            MinutesWork = minutesWork,
                            Date = Convert.ToDateTime(employee.Date.ToString("yyyy-MM-dd 00:00:00")),
                            ETag = "*",
                            PartitionKey = "CONSOLIDATED",
                            RowKey = Guid.NewGuid().ToString()

                        };

                        string filterById = TableQuery.GenerateFilterConditionForInt("EmployeeId", QueryComparisons.Equal, (int)employee.EmployeeId);
                        TableQuery<ConsolidatedEntity> queryEmployee = new TableQuery<ConsolidatedEntity>().Where(filterById);
                        TableQuerySegment<ConsolidatedEntity> employee2 = await consolidatedTable.ExecuteQuerySegmentedAsync(queryEmployee, null);
                        ConsolidatedEntity record = employee2.FirstOrDefault();

                        if (record != null)
                        {
                            record.MinutesWork += minutesWork;
                            TableOperation replaceOperation3 = TableOperation.Replace(record);
                            await consolidatedTable.ExecuteAsync(replaceOperation3);
                        }
                        else
                        {
                            TableOperation addOperation = TableOperation.Insert(consolidatedEntity);
                            await consolidatedTable.ExecuteAsync(addOperation);
                        }

                        employee.Consolidated = true;
                        TableOperation replaceOperation = TableOperation.Replace(employee);
                        await timeRecordTable.ExecuteAsync(replaceOperation);

                        nextEmployee.Consolidated = true;
                        TableOperation replaceOperation2 = TableOperation.Replace(nextEmployee);
                        await timeRecordTable.ExecuteAsync(replaceOperation2);
                    }
                    log.LogInformation($"Consolidated employee record: {employee.EmployeeId}");
                    lp++;

                }
            }
        }
    }
}

