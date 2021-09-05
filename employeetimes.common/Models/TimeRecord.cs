using System;

namespace employeetimes.common.Models
{
    public class TimeRecord
    {
        public int? EmployeeId { get; set; }
        public DateTime? Date { get; set; }
        public int? Type { get; set; }
        public bool Consolidado { get; set; }
    }
}
