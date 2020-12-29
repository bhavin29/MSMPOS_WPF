using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class TableModel
    {
        public int Id { get; set; }
        public int OutletId { get; set; }
        public string TableName { get; set; }
        public int PersonCapacity { get; set; }
        public string TableIcon { get; set; }
        public string Status { get; set; }
    }
}
