using System;
using System.Collections.Generic;
using System.Text;

namespace RocketPOS.Model
{
    public class SyncErrorModel
    {
        public string ErrorNumber { get; set; }
        public string ErrorSeverity { get; set; }
        public string ErrorState { get; set; }
        public string ErrorProcedure { get; set; }
        public string ErrorLine { get; set; }
        public string ErrorMessage { get; set; }
    }
}
