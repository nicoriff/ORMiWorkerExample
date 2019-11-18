using ORMi;
using ORMiWorker.Models;
using ORMiWorker.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ORMiWorker.Services
{
    public class WMIService : IWMIService
    {
        private WMIHelper _helper = new WMIHelper("root/CimV2");

        public async Task<List<Processor>> GetAllProcessors()
        {
            List<Processor> res = (await _helper.QueryAsync<Processor>()).ToList();

            return res;
        }

        public async Task<List<Process>> GetProcesses()
        {
            List<Process> res = (await _helper.QueryAsync<Process>()).ToList();

            return res;
        }
    }
}
