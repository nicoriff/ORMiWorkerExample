using ORMiWorker.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ORMiWorker.Services.Interfaces
{
    public interface IWMIService
    {
        Task<List<Processor>> GetAllProcessors();
        Task<List<Process>> GetProcesses();
    }
}
