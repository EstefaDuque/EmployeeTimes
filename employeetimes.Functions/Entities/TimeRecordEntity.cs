using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace employeetimes.Functions.Entities
{
    public class TimeRecordEntity : TableEntity
    {
        public int? EmployeeId { get; set; }
        public DateTime? Date { get; set; }
        public int? Type { get; set; }
        public bool Consolidado { get; set; }
    }
}
