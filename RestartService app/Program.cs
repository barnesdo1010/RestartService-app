using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RestartService_app
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("To reset EAF 2 tag service, press enter...");
            Console.ReadLine();
            RestartService("TS HSP Ladle Aisle EAF 2 tag service", 60000);
        }
        public static void RestartService(string serviceName, int timeoutMilliseconds)
        {
            EventLog.WriteEntry("F2_Electro-NiteDataServices", string.Format("Restarting EAF2 tag service", EventLogEntryType.Warning));
            ServiceController service = new ServiceController(serviceName);
            try
            {
                Console.WriteLine(service.Status);
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);
                if (service.Status.Equals(ServiceControllerStatus.Running) || (service.Status.Equals(ServiceControllerStatus.StartPending)))
                {
                    Console.WriteLine("Stopping service...");
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                    Console.WriteLine(service.Status);
                    int millisec2 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));
                    Console.WriteLine("Starting service...");
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                else
                {
                    int millisec2 = Environment.TickCount;
                    timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));
                    Console.WriteLine("Starting service...");
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }
                EventLog.WriteEntry("F2_Electro-NiteDataServices", string.Format("EAF2 tag service restarted" + service.Status, EventLogEntryType.Warning));
                Thread.Sleep(1000);
                Console.WriteLine("Service is reset and " + service.Status);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("F2_Electro-NiteDataServices", string.Format("Could not restart EAF2 tag service", EventLogEntryType.Error));
                Console.WriteLine("Could not restart service." + ex);
                Console.ReadLine();
            }
        }
    }
}
