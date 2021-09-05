using System;
using System.Collections.Generic;
using System.Text;

namespace employeetimes.common.Models
{
    public class Consolidated
    {
        public int? EmployeeId { get; set; }
        public DateTime? Date { get; set; }
        public int? MinutesWork { get; set; }
    }
}
