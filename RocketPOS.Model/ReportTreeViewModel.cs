using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class ReportTreeViewModel
    {
        public ReportTreeViewModel()
        {
            Children = new List<ReportTreeViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<ReportTreeViewModel> Children { get; set; }
    }
}
