using employeetimes.common.Models;
using employeetimes.Functions.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json;
using System;
using System.IO;

namespace employeetimes.Test.Helpers
{
    public class TestFactory
    {
        public static TimeRecordEntity GetTimeRecordEntity()
        {
            return new TimeRecordEntity
            {
                
                ETag = "*",
                PartitionKey = "TIMERECORD",
                RowKey = Guid.NewGuid().ToString(),
                EmployeeId = 1,
                Date = DateTime.UtcNow,
                Consolidated = false,
                Type = 0
                
            };
        }

        public static ConsolidatedEntity GetConsolidatedEntity()
        {
            return new ConsolidatedEntity
            {

                ETag = "*",
                PartitionKey = "TIMERECORD",
                RowKey = Guid.NewGuid().ToString(),
                EmployeeId = 1,
                Date = DateTime.UtcNow,
                MinutesWork =180

            };
        }
        public static DefaultHttpRequest CreateHttpRequest(Guid employeekey, TimeRecord timeRecordRequest)
        {
            string request = JsonConvert.SerializeObject(timeRecordRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
                Path = $"/${employeekey}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(DateTime date)
        {
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/${date}"
            };
        }


        public static DefaultHttpRequest CreateHttpRequest(Guid employeekey)
        {

            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Path = $"/${employeekey}"
            };
        }

        public static DefaultHttpRequest CreateHttpRequest(TimeRecord timeRecordRequest)
        {

            string request = JsonConvert.SerializeObject(timeRecordRequest);
            return new DefaultHttpRequest(new DefaultHttpContext())
            {
                Body = GenerateStreamFromString(request),
            };
        }

        public static DefaultHttpRequest CreateHttpRequest()
        {

            return new DefaultHttpRequest(new DefaultHttpContext());

        }

        public static TimeRecord GetTimeRecordRequest()
        {
            return new TimeRecord
            {
                EmployeeId = 1,
                Date = DateTime.UtcNow,
                Consolidated = false,
                Type = 0

            };
        }
        public static Stream GenerateStreamFromString(string stringToConvert)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(stringToConvert);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static ILogger CreateLogger(LoggerTypes type = LoggerTypes.Null)
        {
            ILogger logger;
            if (type == LoggerTypes.List)
            {
                logger = new ListLogger();
            }
            else
            {
                logger = NullLoggerFactory.Instance.CreateLogger("Null Logger");
            }

            return logger;
        }

    }
}
