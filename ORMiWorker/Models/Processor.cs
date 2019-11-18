using ORMi;
using System;
using System.Collections.Generic;
using System.Text;

namespace ORMiWorker.Models
{
    [WMIClass("Win32_Processor")]
    public class Processor
    {
        public string DeviceID { get; set; }
        public string Name { get; set; }
        
        [WMIProperty("LoadPercentage")]
        public int Usage { get; set; }
    }
}
