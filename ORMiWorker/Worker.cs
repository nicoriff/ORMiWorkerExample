using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ORMi;
using ORMiWorker.Models;
using ORMiWorker.Services.Interfaces;

namespace ORMiWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWMIService _wmiService;

        public Worker(ILogger<Worker> logger, IWMIService wmiService)
        {
            _logger = logger;
            _wmiService = wmiService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    List<Process> processes1 = (await _wmiService.GetProcesses()).Where(p => p.Pid != 0).ToList();
                    await Task.Delay(2000);
                    List<Process> processes2 = (await _wmiService.GetProcesses()).Where(p => p.Pid != 0).ToList();


                    List<Process> highCPUConsumingProccesses = new List<Process>();

                    foreach (Process p1 in processes1)
                    {
                        Process p2 = processes2.Where(pr => pr.Pid == p1.Pid).SingleOrDefault();

                        int procDiff = (int)(p2.PercentProcessorTime - p1.PercentProcessorTime);
                        int timeDiff = (int)(p2.TimeStamp_Sys100NS - p1.TimeStamp_Sys100NS);

                        double percentUsage = Math.Round(((double)procDiff / (double)timeDiff) * 100, 2);

                        Console.WriteLine($"{p1.Name}: {percentUsage}%");

                        if (percentUsage > 80)
                        {
                            p1.CpuUsagePercent = percentUsage;
                            highCPUConsumingProccesses.Add(p1);
                        }
                    }

                    if (highCPUConsumingProccesses.Any())
                    {
                        SendMail(System.Environment.GetEnvironmentVariable("COMPUTERNAME"), highCPUConsumingProccesses);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                await Task.Delay(60000, stoppingToken);
            }
        }

        private void SendMail(string hostname, List<Process> processes)
        {
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("your.smtp.com");

            mail.From = new MailAddress("your@mail.com");
            mail.To.Add("sample@mail.com");
            mail.Subject = "Alert: High Consuming CPU";

            StringBuilder sb = new StringBuilder();
            sb.Append("Hi System Administrator:");
            sb.Append("<br />");
            sb.Append("<br />");
            sb.Append($"The hostname: {hostname} is running high on CPU with the following tasks:");
            sb.Append("<br />");

            foreach (Process p in processes)
            {
                sb.Append($"{p.Name}: {p.CpuUsagePercent}%");
            }

            mail.Body = sb.ToString();
            mail.IsBodyHtml = true;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);
            Console.WriteLine("An email has been sent");
        }
    }
}
