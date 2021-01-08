using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class ReportOffsetModel
    {
        public string ReportName { get; set; }
        public string ReportColumn { get; set; }
        public int ColumnOffset { get; set; }
    }
}
