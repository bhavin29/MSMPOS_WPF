using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class TableStatisticsModel
    {
        public string TableName { get; set; }
        public decimal ActualCapacity { get; set; }
        public decimal ExpectedOccupancy { get; set; }
        public decimal Occupancy { get; set; }
        public decimal OccupancyPercentage { get; set; }
    }
}
